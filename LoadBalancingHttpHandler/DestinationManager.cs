// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;

namespace System.Net.Http
{
    public abstract class DestinationManager : IEnumerable<Destination>
    {
        /// <summary>
        /// Get the current target Destinations.
        /// The contents of the returned list should be immutable.
        /// If Destinations are added or removed, a new list should be returned.
        /// </summary>
        /// <returns>The current target Destinations.</returns>
        public abstract IReadOnlyList<Destination> GetCurrentDestinations();

        IEnumerator<Destination> IEnumerable<Destination>.GetEnumerator()
        {
            return GetCurrentDestinations().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetCurrentDestinations().GetEnumerator();
        }
    }
}
