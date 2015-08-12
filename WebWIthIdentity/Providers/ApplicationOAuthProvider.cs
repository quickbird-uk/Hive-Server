﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WebWIthIdentity.Models;

namespace WebWIthIdentity.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            /*Gets Extra keys sent form the user's computer, however we will make this unnessesary
            var data = await context.Request.ReadFormAsync();
            string email = data.GetValues("email").First(); */ 

            //setup nessesary variables
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            string identifier = context.UserName ?? String.Empty;
            ApplicationUser user = null;
            long phone;
            Int64.TryParse(identifier, out phone); 

            if (identifier.Contains("@"))//it's an email!
            {
                user = await userManager.FindByEmailAsync(identifier);
            } 
            else if ( phone > 10000) //it could be a phone number! 
            {
                user = await userManager.Users.Where(p => p.PhoneNumber == phone).FirstOrDefaultAsync(); 
            }
            else if (!String.IsNullOrWhiteSpace(identifier))
            {
                user = await userManager.FindByNameAsync(identifier);
            }

            //user = await userManager.FindAsync(context.UserName, context.Password); //finds the user
            if (user != null)
            {
                string providedPWD = context.Password ?? string.Empty;
                var result = userManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, providedPWD);

                if (result.Equals(PasswordVerificationResult.Success)) //user provided correct creentials
                {
                    ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, //generates identity
                     OAuthDefaults.AuthenticationType);

                    AuthenticationProperties properties = CreateProperties(user.UserName); //generates properties

                    AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties); //puts properties and idenity into authentication ticket

                    context.Validated(ticket);
                }
                else
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect."); //rejects, password is wrong
                }
            }
            else
            {
                context.SetError("invalid_grant", "The user name or password is incorrect."); //rejects, no user found
            }

            /*ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType); 
                */
            //context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            context.TryGetBasicCredentials(out clientId, out clientSecret);
            context.TryGetFormCredentials(out clientId, out clientSecret);

            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }
           

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}