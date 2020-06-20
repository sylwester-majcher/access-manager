﻿using System.Collections.Generic;

namespace Lithnet.AccessManager.Agent
{
    public interface ILapsSettings
    {
        string SigningCertThumbprint { get; }

        bool LapsEnabled { get; }

        int PasswordGenerationStrategy { get; }

        int PasswordLength { get; }

        string PasswordCharacters { get; }

        bool UseUpper { get; }

        bool UseLower { get; }

        bool UseSymbol { get; }

        bool UseNumeric { get; }

        bool UseReadibilitySeparator { get; }

        string ReadabilitySeparator { get; }

        int ReadabilitySeparatorInterval { get; }

        int PasswordHistoryDaysToKeep { get; }

        bool WriteToMsMcsAdmPasswordAttributes { get; }

        int MaximumPasswordAge { get; }

        bool WriteToAppData { get; }
    }
}