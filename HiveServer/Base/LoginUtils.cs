using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using HiveServer.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace HiveServer.Base
{
    public static class LoginUtils
    {
        public static readonly int Interval = 180;
        private static readonly DateTime Epoch = new DateTime(1945, 7, 16);
        private static readonly int TokenLength = 7; 


        public static byte[] hash(string plaintext, byte[] salt)
        {
            SHA512Cng hashFunc = new SHA512Cng();
            byte[] plainBytes = System.Text.Encoding.ASCII.GetBytes(plaintext);
            byte[] toHash = new byte[plainBytes.Length + salt.Length];
            plainBytes.CopyTo(toHash, 0);
            salt.CopyTo(toHash, plainBytes.Length);
            return hashFunc.ComputeHash(toHash);
        }

        public static byte[] generateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[256];
            rng.GetBytes(salt);
            return salt;
        }

        public static bool slowEquals(byte[] a, byte[] b)
        {
            int diff = a.Length ^ b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        public static bool slowEquals(string a, string b)
        {
            int diff = a.Length ^ b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }


        /// <summary>
        /// This method implements something very close to  RFC 6238, but instead of using HMAC, we hash things differently. 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="intervalOffset"></param>
        /// <returns></returns>
        public static string GenerateOTP(ApplicationUser account, int intervalOffset = 0)
        {
            TimeSpan difference = DateTime.UtcNow.Subtract(Epoch); //subtract date of trinity test

            long C = (long)(difference.TotalSeconds / Interval + 0.5d) + intervalOffset; //round the epoch properly

            //By adding user's key acocunt info, we are making sure that a chagne in any of these parameters would invalidate any OTP generated before changes
            string hashable = 
                C + account.Id + account.PhoneNumber 
                + account.PasswordHash ?? string.Empty 
                + account.Email ?? string.Empty 
                + account.FirstName + account.LastName
                + account.SecurityStamp + C;

            byte[] code = hash(hashable, account.OTPSecret);

            //0x1F is the binary number 00001111, which has the upper three bits set to zero,
            //and the lower five bits set to one. AND-ing with this number zeroes out three upper bits.
            int O = code[code.Length -1] & 31; //O is the offset created by 4 lower bits

            UInt64 I = BitConverter.ToUInt64(code, O); 

            string otp = I.ToString();
            
            int toTruncate = otp.Length - TokenLength;

            if (toTruncate > 0)
            { otp = otp.Substring(toTruncate); }
            else if (toTruncate < 0)
            {
                string padding = string.Empty; 
                for (int i = toTruncate; i< 0; i++)
                {
                    padding += "0";
                }
                otp = padding + otp;                 
            }

            return otp; 
        }


        public static bool ValidateOTP(ApplicationUser account, string clientOtp)
        {
            clientOtp = Regex.Replace(clientOtp, @"\s+", "");

            int lowestAllowed = -1; //allows checking if OTP is one of expired ones, in the past
            int highestAllowed = 1; //allows searching OTP into the future, a value of 1 forces searching current values
            bool match = false; 
            
            for(int i = lowestAllowed; i < highestAllowed; i++)
            {
                string serverOtp = Regex.Replace(GenerateOTP(account, i), @"\s+", "");
                if(slowEquals(serverOtp, clientOtp))
                {
                    match = true;
                    break; 
                }
            }

            return match; 
           
        }

        /// <summary>
        /// If the user is found, return the user, otherwise return null
        /// The identifier could be null, then the user will be null
        /// </summary>
        /// <param name="identifier">The identifier provided, it could be a pohone number or email</param>
        /// <param name="userManager">Usermanager from Identity framework</param>
        /// <returns></returns>
        public static async Task<ApplicationUser> findByIdentifierAsync(string identifier, ApplicationUserManager userManager)
        {
            ApplicationUser user = null;
            string email = identifier ?? String.Empty;
            long phone;
            Int64.TryParse(identifier, out phone);

            if (userManager == null)
            {
                throw new NullReferenceException("findByIdentifierAsync enountered null reference in userManager!");
            }
            else
            {
                if (email.Contains("@"))//it's an email!
                {
                    user = await userManager.FindByEmailAsync(identifier);
                }
                else if (phone > 1000000000) //it could be a phone number! 
                {
                    user = await userManager.Users.Where(p => p.PhoneNumber == phone).FirstOrDefaultAsync();
                }
            }

            return user; 
        }

        public static double CalculateShanonPWDEntropy(string password)
        {         
            string input = password;
            double infoC = 0;
            Dictionary<char, double> table = new Dictionary<char, double>();


            foreach (char c in input)
            {
                if (table.ContainsKey(c))
                    table[c]++;
                else
                    table.Add(c, 1);

            }
            double freq;
            foreach (KeyValuePair<char, double> letter in table)
            {

                freq = letter.Value / input.Length; //probability of encountering this letter
                infoC += freq * (Math.Log(freq) / Math.Log(2));
            }

            infoC = Math.Abs(infoC);

            return infoC;          

        }

        public static double CalculateEntropy(string password)
        {
            var cardinality = 0;

            // Password contains lowercase letters.
            if (password.Any(c => char.IsLower(c)))
            {
                cardinality = 26;
            }

            // Password contains uppercase letters.
            if (password.Any(c => char.IsUpper(c)))
            {
                cardinality += 26;
            }

            // Password contains numbers.
            if (password.Any(c => char.IsDigit(c)))
            {
                cardinality += 10;
            }

            // Password contains symbols.
            if (password.IndexOfAny("\\|¬¦`!\"£$%^&*()_+-=[]{};:'@#~<>,./? ".ToCharArray()) >= 0)
            {
                cardinality += 36;
            }

            return Math.Log(cardinality, 2) * password.Length;
        }




    }
}