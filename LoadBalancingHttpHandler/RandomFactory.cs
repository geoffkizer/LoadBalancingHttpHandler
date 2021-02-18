// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace System.Net.Http
{
    internal static class RandomFactory
    {
        public static Random GetRandom()
        {
            // Lame
            // TODO: Make the Random instance thread-static
            return new Random();
        }
    }
}
