﻿using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using Lithnet.AccessManager.Server.Configuration;

namespace Lithnet.AccessManager.Server.Authorization
{
    internal class AuthorizationInformation
    {
        public IUser User { get; set; }

        public IComputer Computer { get; set; }

        public IList<SecurityDescriptorTarget> MatchedTargets { get; set;  } = new List<SecurityDescriptorTarget>();

        public AccessMask EffectiveAccess { get; set; }

        public IList<SecurityDescriptorTarget> FailedTargets { get; set;  } = new List<SecurityDescriptorTarget>();

        public IList<SecurityDescriptorTarget> SuccessfulLapsTargets { get; set; } = new List<SecurityDescriptorTarget>();

        public IList<SecurityDescriptorTarget> SuccessfulLapsHistoryTargets { get; set; } = new List<SecurityDescriptorTarget>();

        public IList<SecurityDescriptorTarget> SuccessfulJitTargets { get; set; } = new List<SecurityDescriptorTarget>();

        public List<GenericSecurityDescriptor> SecurityDescriptors { get; set; } = new List<GenericSecurityDescriptor>();
    }
}
