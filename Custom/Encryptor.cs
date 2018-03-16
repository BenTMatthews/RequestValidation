using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestValidation.Custom
{
    /// <summary>
    // - The method to encrypt or decrypt is not important as long as it is consistant. 
    // - Note that we are using built in key management in this case so it would not work for
    //distributed systems, and a very small case of 90 expiry between encrypt and decrypt 
    //unless we persist the keys.
    /// </summary>
    public class Encryptor
    {
        private readonly IDataProtector _protector;

        public Encryptor(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector(GetType().FullName);
        }

        public string Encrypt(string plaintext)
        {
            return _protector.Protect(plaintext);
        }

        public string Decrypt(string encryptedText)
        {
            return _protector.Unprotect(encryptedText);
        }
    }
}
