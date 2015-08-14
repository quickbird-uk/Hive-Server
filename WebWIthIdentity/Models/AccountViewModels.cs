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
    }


    public class SearchViewModel
    {
        public int Invalid { get; set; }
        public int NotFound { get; set; }
        public int Found{ get; set; }

        public SearchViewModel ()
        {
            FoundContacts = new List<OtherUserInfoViewModel>();
        }

        public List<OtherUserInfoViewModel> FoundContacts { get; set; }
}


    public class OtherUserInfoViewModel
    {
        public string Name { get; set; }

        [DataType(DataType.PhoneNumber)]
        public long Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public Guid ID { get; set; }
    }

    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }

}
