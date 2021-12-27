﻿// OnionFruit API/Tooling Copyright DragonFruit Network
// Licensed under the MIT License. Please refer to the LICENSE file at the root of this project for details

using System.Threading.Tasks;
using DragonFruit.Data;
using DragonFruit.OnionFruit.Api.Objects;
using DragonFruit.OnionFruit.Api.Requests;

namespace DragonFruit.OnionFruit.Api.Extensions
{
    public static class TorStatusClientsExtensions
    {
        public static TorStatusResponse<TorClientConnectionHistory> GetTorBridgeConnectionMetrics(this ApiClient client, int? limit = null, int? offset = null)
        {
            var request = new TorStatusClientsRequest
            {
                Limit = limit,
                Offset = offset
            };

            return client.Perform<TorStatusResponse<TorClientConnectionHistory>>(request);
        }

        public static Task<TorStatusResponse<TorClientConnectionHistory>> GetTorBridgeConnectionMetricsAsync(this ApiClient client, int? limit = null, int? offset = null)
        {
            var request = new TorStatusClientsRequest
            {
                Limit = limit,
                Offset = offset
            };

            return client.PerformAsync<TorStatusResponse<TorClientConnectionHistory>>(request);
        }
    }
}