using System;
using System.Collections.Generic;
using System.Linq;

namespace HiveServer.DTO
{
    public class LoginResult
    {
        public string UserId { get; set; }
        public string MobileServiceAuthenticationToken { get; set; }
        public DateTime ExpirationDate { get; set; }

    }
}