// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;

namespace System.Net.Http
{
    public sealed class RoundRobinLoadBalancingPolicy : LoadBalancingPolicy
    {
        private uint _counter;

        private uint GetCounter()
        {
            return Interlocked.Increment(ref _counter) - 1;
        }

        public override DestinationStatus PickDestination(IReadOnlyList<DestinationStatus> availableDestinations)
        {
            uint counter = GetCounter();

            return availableDestinations[(int)(counter % availableDestinations.Count)];
        }
    }
}
