﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lithnet.AccessManager.Server.UI {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ScriptTemplates {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ScriptTemplates() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Lithnet.AccessManager.Server.UI.ScriptTemplates", typeof(ScriptTemplates).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to # Add-DomainGroupMembershipPermissions
        ///# 
        ///# This script adds the Access Manager service account to the &quot;Windows Authorization Access Group&quot; and &quot;Access Control Assistance Operators&quot; groups in the specified domain
        ///#
        ///# This script requires membership in the Domain Admins group for the domain where permissions need to be added
        ///#
        ///# Version 1.0
        ///
        ///#-------------------------------------------------------------------------
        ///# Do not modify below here
        ///$domain = &quot;{domainDns}&quot;
        ///$serviceAccountSid = &quot;{serviceAc [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string AddDomainGroupMembershipPermissions {
            get {
                return ResourceManager.GetString("AddDomainGroupMembershipPermissions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function Write-AuditLog{
        ///	param(
        ///	[hashtable]$tokens,
        ///	[bool]$isSuccess,
        ///	[Microsoft.Extensions.Logging.ILogger]$logger
        ///)
        ///
        ///	$logger.LogTrace(&quot;We&apos;re in PowerShell for auditing!&quot;);
        ///
        ///	$tokens.Keys | % {
        ///		Write-Host &quot;$($_):$($tokens.Item($_))&quot;;
        ///		$logger.Log(LogLevel.Trace, &quot;$($_):$($tokens.Item($_))&quot;);
        ///		};
        ///}.
        /// </summary>
        internal static string AuditScriptTemplate {
            get {
                return ResourceManager.GetString("AuditScriptTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function Get-AuthorizationResponse{
        ///	param(
        ///	[Lithnet.AccessManager.IUser]$user,
        ///	[Lithnet.AccessManager.IComputer]$computer,
        ///	[Nlog.ILogger]$logger
        ///)
        ///
        ///	$logger.Trace(&quot;We&apos;re in PowerShell!&quot;);
        ///	$logger.Trace(&quot;Checking if $($user.MsDsPrincipalName) is allowed administrative access to $($computer.MsDsPrincipalName)&quot;);
        ///
        ///	$response = New-Object -TypeName &quot;Lithnet.AccessManager.Server.Authorization.PowerShellAuthorizationResponse&quot;
        ///
        ///	# Set IsAllowed to true to allow access, or set IsDenied to explicitl [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string AuthorizationScriptTemplate {
            get {
                return ResourceManager.GetString("AuthorizationScriptTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to # Enable-PamFeature
        ///# 
        ///# This script enabled the &apos;Privileged Access Management&apos; optional feature in a forest
        ///#
        ///# This script requires membership in the Domain Admins group of the forest root domain or Enterprise Admins
        ///#
        ///# Version 1.0
        ///
        ///#-------------------------------------------------------------------------
        ///# Do not modify below here
        ///#-------------------------------------------------------------------------
        ///
        ///Import-Module ActiveDirectory
        ///
        ///$ErrorActionPreference = &quot;Stop&quot;
        ///$InformationPreferen [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string EnablePamFeature {
            get {
                return ResourceManager.GetString("EnablePamFeature", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to # Grant-AccessManagerPermissions
        ///# 
        ///# This script grants permissions for computer objects to write their encrypted password details to the directory, and allows the Lithnet Access Manager service account to read that data
        ///#
        ///# This script requires membership in the Domain Admins group 
        ///#
        ///# Version 1.0
        ///
        ///
        ///#-------------------------------------------------------------------------
        ///# Set the following values as appropriate for your environment
        ///#---------------------------------------------------------- [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GrantAccessManagerPermissions {
            get {
                return ResourceManager.GetString("GrantAccessManagerPermissions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to # Grant-ComputerSelfPermission
        ///# 
        ///# This script grants permissions for computer objects to set their own password attributes
        ///#
        ///# This script requires membership in the Domain Admins group 
        ///# 
        ///#
        ///# Version 1.0
        ///
        ///
        ///#-------------------------------------------------------------------------
        ///# Set the following values as appropriate for your environment
        ///#-------------------------------------------------------------------------
        ///
        ///$OU = &quot;OU=Laps Testing,DC=IDMDEV1,DC=LOCAL&quot;
        ///
        ///#-------------------------- [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GrantComputerSelfPermission {
            get {
                return ResourceManager.GetString("GrantComputerSelfPermission", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to # Grant-GroupPermissions
        ///# 
        ///# This script grants permissions for the AMS service account to create, delete, and manage groups in the specified OU
        ///#
        ///# This script requires membership in the Domain Admins group 
        ///# 
        ///#
        ///# Version 1.0
        ///
        ///#-------------------------------------------------------------------------
        ///# Do not modify below here
        ///#-------------------------------------------------------------------------
        ///
        ///Import-Module ActiveDirectory
        ///
        ///$ErrorActionPreference = &quot;Stop&quot;
        ///$InformationPreference =  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GrantGroupPermissions {
            get {
                return ResourceManager.GetString("GrantGroupPermissions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to # Grant-MsLapsPermissions
        ///# 
        ///# This script grants permissions that allow the Lithnet Admin Access Manager service account to read the Microsoft LAPS passwords from Active Directory
        ///#
        ///# This script requires membership in the Domain Admins group 
        ///#
        ///# Version 1.0
        ///
        ///
        ///#-------------------------------------------------------------------------
        ///# Set the following values as appropriate for your environment
        ///#-------------------------------------------------------------------------
        ///
        ///# Leave this value bla [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GrantMsLapsPermissions {
            get {
                return ResourceManager.GetString("GrantMsLapsPermissions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to # Grant-ServiceAccountPermission
        ///# 
        ///# This script grants permissions for a user or group to read the admin password values, as well as trigger a password reset by cleaning the password expiry field
        ///#
        ///# This script requires membership in the Domain Admins group 
        ///# 
        ///#
        ///# Version 1.0
        ///
        ///
        ///#-------------------------------------------------------------------------
        ///# Set the following values as appropriate for your environment
        ///#-------------------------------------------------------------------------
        ///
        ///$ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GrantServiceAccountPermission {
            get {
                return ResourceManager.GetString("GrantServiceAccountPermission", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to # Publish-LithnetAccessManagerCertificate
        ///# 
        ///# This script creates an object in the Configuration Naming context of the root domain in the forest with a copy
        ///# that contains the public key of the certificate Lithnet Access Manager Agents should use to encrypt their local
        ///# admin passwords and password history
        ///#
        ///# This script requires membership in their the Enterprise Admin group, or the Domain Admin group on the root domain of the forest
        ///# 
        ///# Note, this script has been pre-populated with the inform [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string PublishCertificateTemplate {
            get {
                return ResourceManager.GetString("PublishCertificateTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to # Update-AdSchema
        ///# 
        ///# This script creates the attributes and object classes required to enable encrypted local admin passwords and password history support
        ///# with the Lithnet Access Manager Agent
        ///#
        ///# This script requires membership in their the Schema Admin group
        ///# 
        ///#
        ///# Version 1.0
        ///$ErrorActionPreference = &quot;Stop&quot;
        ///$InformationPreference = &quot;Continue&quot;
        ///
        ///Import-Module ActiveDirectory
        ///
        ///$forest = &quot;{forest}&quot;
        ///$server = (Get-ADDomainController -DomainName $forest -Discover -ForceDiscover -Writable).Ho [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string UpdateAdSchemaTemplate {
            get {
                return ResourceManager.GetString("UpdateAdSchemaTemplate", resourceCulture);
            }
        }
    }
}
