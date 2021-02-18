// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace System.Net.Http
{
    public sealed class FirstLoadBalancingPolicy : LoadBalancingPolicy
    {
        public override DestinationStatus PickDestination(IReadOnlyList<DestinationStatus> availableDestinations)
        {
            return availableDestinations[0];
        }
    }
}
