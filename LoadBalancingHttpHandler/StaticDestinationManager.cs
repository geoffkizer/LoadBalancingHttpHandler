// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace System.Net.Http
{
    public sealed class StaticDestinationManager : DestinationManager
    {
        private bool _inUse;
        private List<Destination> _destinations;

        public StaticDestinationManager()
        {
            _destinations = new List<Destination>();
        }

        public void Add(Destination destination)
        {
            if (_inUse)
            {
                throw new InvalidOperationException();
            }

            _destinations.Add(destination);
        }

        public override IReadOnlyList<Destination> GetCurrentDestinations()
        {
            _inUse = true;

            // Since the set of destinations will not change, we can just return the list directly.
            return _destinations;
        }
    }
}
