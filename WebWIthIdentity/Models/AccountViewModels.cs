using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebWIthIdentity.Models
{
    // Models returned by AccountController actions.

    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class ManageInfoViewModel
    {
        public string LocalLoginProvider { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

    public class UserInfoViewModel
    {
        public string Name { get; set; }

        [DataType(DataType.PhoneNumber)]
        public long Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Twitter { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }

       public List<RecordViewModel> Contacts { get; set; }

        public UserInfoViewModel()
        {
            Contacts = new List<RecordViewModel>();
        }
    }



    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }

}
