﻿using System.ComponentModel.DataAnnotations;

namespace Lithnet.AccessManager.Configuration
{
    public class JsonFileTargetsProviderOptions
    {
        public string AuthorizationFile { get; set; }

        public bool Enabled { get; set; } = true;
    }
}
