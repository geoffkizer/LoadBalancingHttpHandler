// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace System.Net.Http
{
    public sealed class LeastRequestsLoadBalancingPolicy : LoadBalancingPolicy
    {
        public override DestinationStatus PickDestination(IReadOnlyList<DestinationStatus> availableDestinations)
        {
            DestinationStatus chosenDestination = availableDestinations[0];
            int leastRequestCount = chosenDestination.CurrentRequestCount;
            for (int i = 1; i < availableDestinations.Count; i++)
            {
                int destinationRequestCount = availableDestinations[i].CurrentRequestCount;
                if (destinationRequestCount < leastRequestCount)
                {
                    chosenDestination = availableDestinations[i];
                    leastRequestCount = destinationRequestCount;
                }
            }

            return chosenDestination;
        }
    }
}
