﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lithnet.AccessManager.Server.Configuration
{
    public class UserInterfaceOptions
    {
        public string Title { get; set; } = "Lithnet Access Manager";

        [JsonConverter(typeof(StringEnumConverter))]
        public AuditReasonFieldState UserSuppliedReason { get; set; } = AuditReasonFieldState.Optional;

        public PhoneticSettings PhoneticSettings { get; set; } = new PhoneticSettings();
    }
}