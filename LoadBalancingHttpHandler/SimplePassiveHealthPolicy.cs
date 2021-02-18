// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace System.Net.Http
{
    public sealed class SimplePassiveHealthPolicy : PassiveHealthPolicy
    {
        public TimeSpan ObservationPeriod { get; init; }

        public int FailureThreshold { get; init; }

        public TimeSpan DeactivationPeriod { get; init; }

        public override TimeSpan? CheckForDeactivationOnFailure(DestinationStatus failedDestinationStatus)
        {
            // TODO: Actually implement ObservationPeriod and FailureThreshold, and do it per-Destination
            // Right now, we always deactivate on the first failure
            return DeactivationPeriod;
        }
    }
}
