﻿using System.Security.Claims;
using Lithnet.AccessManager.Configuration;
using Lithnet.AccessManager.Web.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog;

namespace Lithnet.AccessManager.Web.AppSettings
{
    public class WsFedAuthenticationProvider : IdpAuthenticationProvider, IWsFedAuthenticationProvider
    {
        private readonly WsFedAuthenticationProviderOptions options;

        public WsFedAuthenticationProvider(IOptions<WsFedAuthenticationProviderOptions> options, ILogger logger, IDirectory directory, IHttpContextAccessor httpContextAccessor)
            :base (logger, directory, httpContextAccessor)
        {
            this.options = options.Value;
        }
        
        public override bool CanLogout => true;

        public override bool IdpLogout => this.options.IdpLogout;

        protected override string ClaimName => this.options.ClaimName;

        public override void Configure(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddWsFederation("laps", options =>
            {
                options.CallbackPath = "/auth";
                options.MetadataAddress = this.options.Metadata;
                options.Wtrealm = this.options.Realm;
                options.Events = new WsFederationEvents()
                {
                    OnSecurityTokenValidated = this.FindClaimIdentityInDirectoryOrFail,
                    OnAccessDenied = this.HandleAuthNFailed,
                    OnRemoteFailure = this.HandleRemoteFailure
                };
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Home/Login";
                options.LogoutPath = "/Home/SignOut";
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
        }
    }
}