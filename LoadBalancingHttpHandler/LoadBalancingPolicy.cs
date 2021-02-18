// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace System.Net.Http
{
    public abstract class LoadBalancingPolicy
    {
        public abstract DestinationStatus PickDestination(IReadOnlyList<DestinationStatus> availableDestinations);
    }
}
