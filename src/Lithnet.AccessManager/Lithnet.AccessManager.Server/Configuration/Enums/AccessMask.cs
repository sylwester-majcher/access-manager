﻿using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lithnet.AccessManager.Server.Configuration
{
    [JsonConverter(typeof(StringEnumConverter))]
    [Flags]
    public enum AccessMask
    {
        None = 0,

        [Description("Active local admin password")]
        Laps = 0x200,

        [Description("Previous local admin passwords")]
        LapsHistory = 0x400,

        [Description("Just-in-time access")]
        Jit = 0x800,
    }
}
