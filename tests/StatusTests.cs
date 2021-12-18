﻿// OnionFruit API/Tooling Copyright DragonFruit Network
// Licensed under the MIT License. Please refer to the LICENSE file at the root of this project for details

using System.Threading.Tasks;
using DragonFruit.OnionFruit.Api.Status.Enums;
using DragonFruit.OnionFruit.Api.Status.Extensions;
using NUnit.Framework;

namespace DragonFruit.OnionFruit.Api.Tests
{
    [TestFixture]
    public class StatusTests : OnionFruitApiTest
    {
        [Test]
        public async Task TestSummary()
        {
            var summary = await Client.GetTorSummaryAsync(500);
            Assert.AreEqual(500, summary.Relays.Length);
        }

        [Test]
        public async Task TestBridgeSummary()
        {
            var summary = await Client.GetTorSummaryAsync(200, type: TorNodeType.Bridge);
            Assert.AreEqual(200, summary.Bridges.Length);
        }

        [Test]
        public async Task TestDetails()
        {
            var details = await Client.GetTorDetailsAsync(250);
            Assert.AreEqual(250, details.Relays.Length);
        }

        [Test]
        public async Task TestBandwidthHistory()
        {
            var history = await Client.GetBandwidthHistoryAsync(750);
            Assert.AreEqual(history.Relays.Length, 750);
        }

        [Test]
        public async Task TestBridgeConnectionMetrics()
        {
            var history = await Client.GetTorBridgeConnectionMetricsAsync(100);
            Assert.AreEqual(history.Bridges.Length, 100);
        }
    }
}
