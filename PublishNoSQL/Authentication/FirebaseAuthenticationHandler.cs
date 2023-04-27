﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace PublishNoSQL.Authentication
{
    public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string BEARER_PREFIX = "Bearer ";


        public FirebaseAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Context.Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            string bearerToken = Context.Request.Headers["Authorization"];

            if (bearerToken == null || !bearerToken.StartsWith(BEARER_PREFIX))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid scheme."));
            }

            string token = bearerToken.Substring(BEARER_PREFIX.Length);
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            try
            {

                return Task.FromResult(AuthenticateResult.Success(CreateAuthenticationTicket(jwtToken)));
            }
            catch (Exception ex)
            {
                return Task.FromResult(AuthenticateResult.Fail(ex));
            }
        }

        private AuthenticationTicket CreateAuthenticationTicket(JwtSecurityToken firebaseToken)
        {
            //ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>()
            //{
            //    new ClaimsIdentity(ToClaims(firebaseToken.Claims.All), nameof(ClaimsIdentity))
            //});
            string role = firebaseToken.Claims.First(c => c.Type == "role").Value;

            var additionalClaims = new List<Claim>
            {
                new Claim("role", role)
            };

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new ClaimsIdentity(additionalClaims, nameof(ClaimsIdentity))
            });

            return new AuthenticationTicket(claimsPrincipal, JwtBearerDefaults.AuthenticationScheme);
        }

    }
}
