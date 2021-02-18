// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Threading;

namespace System.Net.Http
{
    // CONSIDER: Rename to ActiveDestination?
    public sealed class DestinationStatus
    {
        private int _currentRequestCount;

        internal DestinationStatus(Destination destination)
        {
            Destination = destination;
            IsHealthy = true;
            _currentRequestCount = 0;
        }

        public Destination Destination { get; init; }

        public bool IsHealthy { get; internal set; }

        public int CurrentRequestCount => _currentRequestCount;

        internal void IncrementRequestCount()
        {
            Debug.Assert(_currentRequestCount >= 0);
            Interlocked.Increment(ref _currentRequestCount);
        }

        internal void DecrementRequestCount()
        {
            Interlocked.Decrement(ref _currentRequestCount);
            Debug.Assert(_currentRequestCount >= 0);
        }
    }
}
