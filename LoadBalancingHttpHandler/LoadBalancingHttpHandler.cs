// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace System.Net.Http
{
    public sealed class LoadBalancingHttpHandler : DelegatingHandler
    {
        private IReadOnlyList<Destination> _currentDestinations;
        private List<DestinationStatus> _destinationStatuses;

        public LoadBalancingHttpHandler(HttpMessageHandler innerHandler) 
            : base(innerHandler)
        {
        }

        private object LockObject { get; } = new object();

        public DestinationManager DestinationManager { get; init; }

        public LoadBalancingPolicy LoadBalancingPolicy { get; init; }

        public PassiveHealthPolicy PassiveHealthPolicy { get; init; }

        private IReadOnlyList<DestinationStatus> GetDestinationStatuses()
        {
            if (_currentDestinations != DestinationManager.GetCurrentDestinations())
            {
                lock (LockObject)
                {
                    IReadOnlyList<Destination> newDestinations = DestinationManager.GetCurrentDestinations();
                    if (_currentDestinations != newDestinations)
                    {
                        // Destination definition has changed.
                        // Flush current status and rebuild.
                        // TODO: This is kind of lame because I toss previous status info, but I think it's reasonable at least for now.
                        List<DestinationStatus> newDestinationStatuses = new List<DestinationStatus>(newDestinations.Count);

                        for (int i = 0; i < newDestinations.Count; i++)
                        {
                            newDestinationStatuses.Add(new DestinationStatus(newDestinations[i]));
                        }

                        _currentDestinations = newDestinations;
                        _destinationStatuses = newDestinationStatuses;
                        return newDestinationStatuses;
                    }
                }
            }

            return _destinationStatuses;
        }

        private DestinationStatus PickDestination()
        {
            return LoadBalancingPolicy.PickDestination(GetDestinationStatuses());
        }

        private async Task ReactivateDestinationAsync(DestinationStatus destinationStatus, TimeSpan deactivationPeriod)
        {
            await Task.Delay(deactivationPeriod);

            // TODO: This isn't properly synchronizing with changes in the underlying destination set
            lock (LockObject)
            {
                List<DestinationStatus> newDestinationStatuses = new List<DestinationStatus>(_destinationStatuses);
                newDestinationStatuses.Add(destinationStatus);

                _destinationStatuses = newDestinationStatuses;
            }
        }

        private void HandleRequestFailure(DestinationStatus destinationStatus)
        {
            TimeSpan? deactivationPeriod = PassiveHealthPolicy.CheckForDeactivationOnFailure(destinationStatus);
            if (deactivationPeriod is not null)
            {
                // TODO: This is kind of lame.
                // Instead of having a reference from DestinationStatus to Destination, 
                // consider holding the current Destination set and an index into it.

                destinationStatus.IsHealthy = false;

                // TODO: This isn't properly synchronizing with changes in the underlying destination set
                lock (LockObject)
                {
                    List<DestinationStatus> newDestinationStatuses = new List<DestinationStatus>(_destinationStatuses);
                    newDestinationStatuses.Remove(destinationStatus);

                    _destinationStatuses = newDestinationStatuses;
                }

                _ = ReactivateDestinationAsync(destinationStatus, deactivationPeriod.Value);
           }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Handle retry on failed request
            while (true)
            {
                DestinationStatus destinationStatus = PickDestination();
                Destination destination = destinationStatus.Destination;

                // CONSIDER: I'm just always ignoring the original hostname on the request. Is that the right thing to do?
                request.RequestUri = new Uri(destination.Uri, request.RequestUri!.PathAndQuery);

                destinationStatus.IncrementRequestCount();
                try
                {
                    return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    // TODO: seems like there should be a threshold for failure so we don't keep retrying forever...
                    HandleRequestFailure(destinationStatus);
                }
                finally
                {
                    destinationStatus.DecrementRequestCount();
                }
            }
        }
    }
}
