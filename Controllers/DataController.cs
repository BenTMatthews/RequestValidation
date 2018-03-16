using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RequestValidation.Custom;

namespace RequestValidation.Controllers
{
    [Produces("application/json")]
    [Route("api/Data")]
    public class DataController : Controller
    {
        //DI injected, framework manage envryption provider
        private readonly IDataProtectionProvider _encryptorProvider;

        public DataController(IDataProtectionProvider encryptorProvider)
        {
            _encryptorProvider = encryptorProvider;
        }

        /// <summary>
        /// This will take the val to encrypt
        /// </summary>
        /// <param name="val">To be encrypted</param>
        /// <returns>The encrypted string, and the key used to do so</returns>
        [HttpGet]
        [Route("EncryptString")]
        public ServiceResponse<string[]> EncryptString(string val)
        {
            ServiceResponse<string[]> resp = new ServiceResponse<string[]>();

            try
            {
                //validate inputs
                if (!string.IsNullOrEmpty(val))
                {
                    long key = DateTime.Now.Ticks;
                    string salt = (key / 2).ToString();

                    Encryptor encryptor = new Encryptor(_encryptorProvider);

                    int halfMeasure = salt.Length / 2;
                    val = salt.Substring(0, halfMeasure) + val;
                    val += salt.Substring(halfMeasure, salt.Length - halfMeasure);

                    resp.Message = "Success";
                    resp.Success = true;
                    resp.Data = new string[] { encryptor.Encrypt(val), key.ToString() };
                }
                else
                {
                    resp.Data = null;
                    resp.Message = "Invalid inputs";
                    resp.Success = false;
                }
            }
            catch (Exception ex)
            {
                resp.Data = null;
                resp.Message = ex.Message;
                resp.Success = false;
            }

            return resp;
        }

        [HttpGet]
        [Route("DecryptString")]
        public ServiceResponse<string> DecryptString(string val, string key)
        {
            long keyParsed;
            ServiceResponse<string> resp = new ServiceResponse<string>();

            try
            {
                if (!string.IsNullOrEmpty(val) && long.TryParse(key, out keyParsed) && keyParsed > 100)
                {
                    string salt = (keyParsed / 2).ToString();
                    int halfMeasure = salt.Length / 2;
                    string frontSalt = salt.Substring(0, halfMeasure);
                    string backSalt = salt.Substring(halfMeasure, salt.Length - halfMeasure);

                    Encryptor encryptor = new Encryptor(_encryptorProvider);

                    string decryptedVal = encryptor.Decrypt(val);

                    if((decryptedVal.IndexOf(frontSalt) >= 0) && (decryptedVal.IndexOf(backSalt) >= 0))
                    {
                        string unsaltedDecryptVal = decryptedVal.Replace(frontSalt, "").Replace(backSalt, "");

                        resp.Data = unsaltedDecryptVal;
                        resp.Message = "Success";
                        resp.Success = true;
                    }
                    else
                    {
                        resp.Data = null;
                        resp.Message = "Decrypting failed, missing salts";
                        resp.Success = false;
                    }
                }
                else
                {
                    resp.Data = null;
                    resp.Message = "Invalid inputs";
                    resp.Success = false;
                }
            }
            catch (Exception ex)
            {
                resp.Data = null;
                resp.Message = ex.Message;
                resp.Success = false;
            }

            return resp;

        }
    }
}
