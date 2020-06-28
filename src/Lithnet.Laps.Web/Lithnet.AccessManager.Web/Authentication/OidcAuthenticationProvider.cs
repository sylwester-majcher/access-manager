﻿using System.Security.Claims;
using Lithnet.AccessManager.Configuration;
using Lithnet.AccessManager.Web.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using NLog;

namespace Lithnet.AccessManager.Web.AppSettings
{
    public class OidcAuthenticationProvider : IdpAuthenticationProvider, IOidcAuthenticationProvider
    {
        private readonly OidcAuthenticationProviderOptions options;

        public OidcAuthenticationProvider(IOptions<OidcAuthenticationProviderOptions> options, ILogger logger, IDirectory directory, IHttpContextAccessor httpContextAccessor)
            : base(logger, directory, httpContextAccessor)
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
             .AddOpenIdConnect("laps", options =>
             {
                 options.Authority = this.options.Authority;
                 options.ClientId = this.options.ClientID;
                 options.ClientSecret = this.options.Secret;
                 options.CallbackPath = "/auth";
                 options.SignedOutCallbackPath = "/auth/logout";
                 options.SignedOutRedirectUri = "/Home/LoggedOut";
                 options.ResponseType = this.options.ResponseType;
                 options.Scope.Clear();
                 options.Scope.Add("openid");
                 options.Scope.Add("profile");
                 options.SaveTokens = true;
                 options.GetClaimsFromUserInfoEndpoint = true;
                 options.UseTokenLifetime = true;

                 options.Events = new OpenIdConnectEvents()
                 {
                     OnTokenValidated = this.FindClaimIdentityInDirectoryOrFail,
                     OnRemoteFailure = this.HandleRemoteFailure,
                     OnAccessDenied = this.HandleAuthNFailed,
                 };
             })
             .AddCookie(options =>
             {
                 options.LoginPath = "/Home/Login";
                 options.LogoutPath = "/Home/Logout";
             });
        }
    }
}