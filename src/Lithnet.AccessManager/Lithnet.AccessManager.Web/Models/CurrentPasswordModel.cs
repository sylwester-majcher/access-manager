﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Lithnet.AccessManager.Web.Models
{
    public class CurrentPasswordModel
    {
        [Required]
        public string ComputerName { get; set; }

        public string Password { get; set; }

        public DateTime? ValidUntil { get; set; }
    }
}