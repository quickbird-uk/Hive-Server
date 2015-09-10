using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using HiveServer.Models;

namespace HiveServer.Base
{
	public class LoginUtils
	{
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

        //Generates an OTP that stays active for 5 minutes
        public static string GenerateOTP5min(ApplicationUser account)
        {            
            
            TimeSpan difference = DateTime.UtcNow.Subtract(account.LastSMSCode);
            if (difference > new TimeSpan(0, 5, 0))
            {
                account.updateLastSMSCode();
            }
            string hashable = account.Id + account.LastSMSCode.ToString() + account.PhoneNumber;

            byte[] code = hash(hashable, account.OTPSalt);

            string otp = string.Empty;

            int[] ms = new int[8];

            for(int i=0; i < code.Length; i++) //suming together chunks of the hash, not very usefull but somewhat amusing
            {
                int pos = (int)(((float) i / (float) code.Length) * ms.Length);
                ms[pos] += code[i]; 
            }
            //making a number string with a space in the middle
            for (int i = 0; i < 3; i++)
            {
                otp += (ms[i] % 10);
            }

            otp += " ";

            for (int i = 3; i < 7; i++)
            {
                otp += (ms[i] % 10);
            }
            
            return otp; 
        }

    }
}