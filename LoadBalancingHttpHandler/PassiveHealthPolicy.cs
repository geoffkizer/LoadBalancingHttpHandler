// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace System.Net.Http
{
    public abstract class PassiveHealthPolicy
    {
        // TODO: This is pretty lame because there's no way to track passive history and 
        // make a determination based on that.
        // For now it's fine though.
        // Return value: null means don't deactivate, otherwise TimeSpan indicates how long to deactivate for.
        public abstract TimeSpan? CheckForDeactivationOnFailure(DestinationStatus failedDestinationStatus);
    }
}
