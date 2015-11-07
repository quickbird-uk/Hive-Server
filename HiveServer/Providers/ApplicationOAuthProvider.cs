using System;
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
using HiveServer.Models;
using System.Text.RegularExpressions;
using HiveServer.DTO;
using HiveServer.Base; 

namespace HiveServer.Providers
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
            ApplicationUser user = null;            
            bool loggedIn = false;

            
            if (string.IsNullOrWhiteSpace(context.Password)) //check if valid password was provided
            {
                context.SetError(ErrorResponse.PasswordIsBad.error.ToString(), ErrorResponse.PasswordIsBad.error_description); //rejects, password is wrong
                return; 
            }
            else
            { user = await LoginUtils.findByIdentifierAsync(context.UserName, userManager); }

            
            
            if (user == null)
            {
                context.SetError(ErrorResponse.CantLogin.error.ToString(), ErrorResponse.CantLogin.error_description); //rejects, no user found
                return;
            }
                        
            var PasswordCheck = userManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, context.Password);  //check if password was correct

            if (PasswordCheck.Equals(PasswordVerificationResult.Success) && user.PhoneNumberConfirmed) //user provided correct creentials
            {
                loggedIn = true;
            }
            else if(PasswordCheck.Equals(PasswordVerificationResult.Success))
            {
                //Send an SMS!!
                string otp = Base.LoginUtils.GenerateOTP(user);
                await SMSService.SendMessage(user.PhoneNumber.ToString(), String.Format("Your SMS code is {0} use it to confirm your phone number withing {1} min", otp, LoginUtils.Interval / 60));
                context.SetError(ErrorResponse.PhoneNumberUnconfirmed.error.ToString(), ErrorResponse.PhoneNumberUnconfirmed.error_description);
            }            
            else if (!loggedIn)// log in with SMS code
            {
                loggedIn = LoginUtils.ValidateOTP(user, context.Password);

                if (!user.PhoneNumberConfirmed && loggedIn) //if the user's phone number is not confirmed, and if logged in set it confirmed
                {
                    user.PhoneNumberConfirmed = true;
                    await userManager.UpdateAsync(user);
                }
            }
             

            if (loggedIn) //user provided correct creentials
            {
                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, //generates identity
                 OAuthDefaults.AuthenticationType);

                AuthenticationProperties properties = CreateProperties(user.Id.ToString()); //generates properties

                AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties); //puts properties and idenity into authentication ticket

                context.Validated(ticket);
            }
            else
            {
                context.SetError(ErrorResponse.CantLogin.error.ToString(), ErrorResponse.CantLogin.error_description); //rejects, password is wrong
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

        public static AuthenticationProperties CreateProperties <T>(T userId)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "useId", userId.ToString() }
            };
            return new AuthenticationProperties(data);
        }
    }
}