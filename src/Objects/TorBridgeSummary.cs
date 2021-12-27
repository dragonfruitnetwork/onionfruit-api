﻿// OnionFruit API/Tooling Copyright DragonFruit Network
// Licensed under the MIT License. Please refer to the LICENSE file at the root of this project for details

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DragonFruit.OnionFruit.Api.Objects
{
    [Serializable, DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    public class TorBridgeSummary
    {
        /// <summary>
        /// The relay nickname, represented as 1-19 chars
        /// </summary>
        [DataMember(Name = "n")]
        public string Nickname { get; set; }

        /// <summary>
        /// SHA-1 hash of the bridge fingerprint
        /// </summary>
        [DataMember(Name = "h")]
        public string Fingerprint { get; set; }

        /// <summary>
        /// Whether the relay was running at the last consensus
        /// </summary>
        [DataMember(Name = "r")]
        public bool Running { get; set; }
    }
}