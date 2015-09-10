using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WebGrease.Css.Extensions;
using HiveServer.Models;
using HiveServer.Providers;
using HiveServer.Results;
using HiveServer.DTO;

namespace HiveServer.Controllers
{
    [Authorize]
    [RoutePrefix("Account")]
    public class AccountController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }


        ///<summary>Retrieves general information about the user's account.</summary>
        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [ResponseType(typeof(Account))]
        [Route("UserInfo")]
        public async Task<IHttpActionResult> GetUserInfo()
        {
            //ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            var UserId = Int64.Parse(User.Identity.GetUserId()); 

            var thisUser =
                await
                    UserManager.Users.Where(p => p.Id == UserId).FirstOrDefaultAsync();


            if (thisUser != null)
            {


                var uView = new Account
                {
                    lastName = thisUser.LastName,
                    firstName = thisUser.FirstName,
                    phone = thisUser.PhoneNumber,
                };

                return Ok(uView);
            }
            else
            {
                return NotFound();
            }
            
        }

        ///<summary>Changes user's Email. Eventually it will even require user to confirm it.</summary>
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("ChangeEmail")]
        public async Task<IHttpActionResult> ChangeEmail(ChangeEmailBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            var thisUser = await UserManager.FindByIdAsync(Int64.Parse(User.Identity.GetUserId()));
            thisUser.Email = model.Email;
            await UserManager.UpdateAsync(thisUser);
            return Ok(); 
        }

        ///<summary>Changes user's phone number. Eventually it will even require user to confirm it.</summary>
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("ChangePhone")]
        public async Task<IHttpActionResult> ChangePhone(ChangePhoneBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var thisUser = await UserManager.FindByIdAsync(Int64.Parse(User.Identity.GetUserId()));
            thisUser.PhoneNumber = model.PhoneNumber;
            await UserManager.UpdateAsync(thisUser);
            return Ok();
        }

        /////<summary>Changes miscaleneous fields on the user's account, those of secondary importance</summary>
        //[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        //[Route("ChangeMisc")]
        //public async Task<IHttpActionResult> ChangeMisc(ChangeMiscBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var thisUser = await UserManager.FindByIdAsync(Int64.Parse(User.Identity.GetUserId()));
        //    if (!string.IsNullOrWhiteSpace(model.f))
        //        thisUser.FirstName = model.Name;
        //    if (!string.IsNullOrWhiteSpace(model.Twitter))
        //        thisUser.Twitter = model.Twitter;
           
        //    await UserManager.UpdateAsync(thisUser);
        //    return Ok();
        //}

        
        // POST api/Account/Logout
        ///<summary>Disregard</summary>
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET Account/ManageInfo?returnUrl=%2F&generateState=true
        ///<summary>Disregard</summary>
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(Int64.Parse(User.Identity.GetUserId()));

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (var linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }




        ///<summary>Use this method to set change password on a user's account</summary>
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(Int64.Parse(User.Identity.GetUserId()), model.OldPassword,
                model.NewPassword);
            
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        ///<summary>Disregard</summary>
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(RegistrationRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(Int64.Parse(User.Identity.GetUserId()), model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        ///<summary>Disregard</summary>
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(Int64.Parse(User.Identity.GetUserId()),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        ///<summary>Disregard</summary>
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(Int64.Parse(User.Identity.GetUserId()));
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(Int64.Parse(User.Identity.GetUserId()),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        ///<summary>Disregard</summary>
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                
                 ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        ///<summary>Disregard</summary>
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        ///<summary>Creates a new user with Local authentication</summary>
        [AllowAnonymous]
        [Route("Register")]  
        public async Task<IHttpActionResult> Register(RegistrationRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (model == null)
            {
                return BadRequest("You sent no data");
            }
            else
            {
                
                var user = new ApplicationUser()
                {
                    PhoneNumber = model.phone,
                    FirstName = model.firstName,
                    LastName = model.lastName, 
                    OTPSalt = Base.LoginUtils.generateSalt()

                }; //create user

                user.UserName = Guid.NewGuid().ToString(); //give the USername a value, for soem reason

                IdentityResult result =  new IdentityResult(); 

                try
                {
                    // Your code...
                    // Could also be before try if you know the exception occurs in SaveChanges

                    result = await UserManager.CreateAsync(user, model.Password);

                    string otp = Base.LoginUtils.GenerateOTP5min(user);
                    await SMSService.SendMessage(model.phone.ToString(), "Welcome to Hive, your code is " + otp + " use it to confirm your phone number within 5 min");
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        ModelState.AddModelError("Entity of type " 
                            + eve.Entry.Entity.GetType().Name, "in state " + eve.Entry.State + " has the following validation errors");

                        foreach (var ve in eve.ValidationErrors)
                        {
                            ModelState.AddModelError(ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                }
                    

                if (!result.Succeeded)
                {
                    return BadRequest(ModelState);
                }

                return Ok();
                

            }
            
        }

        // POST api/Account/RegisterExternal
        ///<summary>Disregard</summary>
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result); 
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers


        private static async Task<bool> IsEmailUnique(ApplicationUserManager userManager, string email)
        {
            var list = await userManager.Users.Where(p => p.Email.Equals(email)).ToListAsync();
            
            if (list.Count == 0)
            {   return true;}
            else
            {   return false;}
        }

        private static async Task<bool> IsPhoneUnique(ApplicationUserManager userManager, long phone)
        {
            var list = await userManager.Users.Where(p => p.PhoneNumber == phone).ToListAsync();

            if (list.Count == 0)
            { return true; }
            else
            { return false; }
        }


        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }


        


        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
