// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace System.Net.Http
{
    public sealed class PowerOfTwoLoadBalancingPolicy : LoadBalancingPolicy
    {
        public override DestinationStatus PickDestination(IReadOnlyList<DestinationStatus> availableDestinations)
        {
            Random r = RandomFactory.GetRandom();

            DestinationStatus first = availableDestinations[r.Next(availableDestinations.Count)];
            DestinationStatus second = availableDestinations[r.Next(availableDestinations.Count)];
            return (first.CurrentRequestCount <= second.CurrentRequestCount) ? first : second;
        }
    }
}
