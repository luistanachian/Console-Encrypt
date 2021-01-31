using System;
using System.Security.Cryptography;
using System.Text;

namespace EncryptTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            string[] keys = new string[] {
                "my dog ran away from home and i can't get it anywhere",
                "the cat licks its paw and tail"
            };
            Console.WriteLine("Test of Encrypt with MD5");

            foreach (var key in keys)
            {
                Console.WriteLine("------------------------");
                Console.WriteLine($"Key => {key}");
                var text = "Hello Wolrd!";
                Console.WriteLine($"Text to encrypt => {text}");

                var encrypted = Security.Encrypt_MD5(text, key);
                Console.WriteLine($"Text encrypted => {encrypted}");

                var decrypted = Security.Decrypt_MD5(encrypted, key);
                Console.WriteLine($"Text decrypted => {decrypted}");

                var incorrectKey = "Say my name!";
                var decryptedError = Security.Decrypt_MD5(encrypted, incorrectKey);
                Console.WriteLine($"Wrong key => {incorrectKey}");
                Console.WriteLine($"Text decrypted, with wrong key => {decryptedError}");
            }
            Console.Read();
        }
    }

    public static class Security
    {
        public static string Encrypt_MD5(string text, string key)
        {
            byte[] arrayToEncrypt = Encoding.UTF8.GetBytes(text);
            var hashmd5 = new MD5CryptoServiceProvider();
            byte[] keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();

            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(arrayToEncrypt, 0, arrayToEncrypt.Length);
            tdes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt_MD5(string textoEncriptado, string key)
        {
            try
            {
                byte[] arraytoDecrypt = Convert.FromBase64String(textoEncriptado);
                var hashmd5 = new MD5CryptoServiceProvider();
                byte[] keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();

                var tdes = new TripleDESCryptoServiceProvider
                {
                    Key = keyArray,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(arraytoDecrypt, 0, arraytoDecrypt.Length);
                tdes.Clear();

                return Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception)
            {
                return "incorrect data";
            }
        }
    }
}
