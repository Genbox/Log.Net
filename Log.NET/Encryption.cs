using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LogNET
{
    public static class Encryption
    {
        #region Encrypt

        /// <summary>
        /// Encrypts the specified clear data.
        /// </summary>
        /// <param name="clearData">The clear data.</param>
        /// <param name="Key">The key.</param>
        /// <param name="IV">The IV.</param>
        /// <returns></returns>
        private static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            Rijndael algorithm = Rijndael.Create();
            algorithm.Key = Key;
            algorithm.IV = IV;

            if (ConfigurationManager.AppSettings["EncryptionBlockSize"] != null)
            {
                int blocksize = int.Parse(ConfigurationManager.AppSettings["EncryptionBlockSize"], NumberStyles.Integer, CultureInfo.InvariantCulture);
                if (blocksize != 128 && blocksize != 192 && blocksize != 256)
                    throw new ConfigurationErrorsException("Please provide a block size. Supported block size is: 128, 192, or 256 bits.");

                algorithm.BlockSize = blocksize;
            }
            else
            {
                algorithm.BlockSize = 128;
            }

            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, algorithm.CreateEncryptor(), CryptoStreamMode.Write);

            cryptoStream.Write(clearData, 0, clearData.Length);
            cryptoStream.Close();

            byte[] encryptedData = memoryStream.ToArray();
            return encryptedData;
        }

        /// <summary>
        /// Encrypts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string Encrypt(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                byte[] clearBytes = Encoding.Unicode.GetBytes(data);

                if (ConfigurationManager.AppSettings["EncryptionPassword"] != null && ConfigurationManager.AppSettings["EncryptionSalt"] != null)
                {
                    string password = ConfigurationManager.AppSettings["EncryptionPassword"];

                    byte[] salt = new ASCIIEncoding().GetBytes(ConfigurationManager.AppSettings["EncryptionSalt"]);

                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt);

                    byte[] encPwd = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
                    return Convert.ToBase64String(encPwd);
                }
                throw new ConfigurationErrorsException("You need to configure the encryption password and salt in the configuration file");
            }
            throw new ArgumentNullException("data", "The data to encrypt is empty");
        }

        #endregion

        #region Decrypt

        /// <summary>
        /// Decrypts the specified cipher data.
        /// </summary>
        /// <param name="cipherData">The cipher data.</param>
        /// <param name="Key">The key.</param>
        /// <param name="IV">The IV.</param>
        /// <returns></returns>
        private static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            MemoryStream memoryStream = new MemoryStream();

            Rijndael algorithm = Rijndael.Create();
            algorithm.Key = Key;
            algorithm.IV = IV;

            if (ConfigurationManager.AppSettings["EncryptionBlockSize"] != null)
            {
                int blocksize = int.Parse(ConfigurationManager.AppSettings["EncryptionBlockSize"], NumberStyles.Integer, CultureInfo.InvariantCulture);
                if (blocksize != 128 && blocksize != 192 && blocksize != 256)
                    throw new ConfigurationErrorsException("Please provide a block size. Supported block size is: 128, 192, or 256 bits.");

                algorithm.BlockSize = blocksize;
            }
            else
            {
                algorithm.BlockSize = 128;
            }

            CryptoStream cryptoStream = new CryptoStream(memoryStream, algorithm.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(cipherData, 0, cipherData.Length);
            cryptoStream.FlushFinalBlock();
            cryptoStream.Close();

            byte[] decryptedData = memoryStream.ToArray();
            return decryptedData;
        }

        /// <summary>
        /// Decrypts the specified encrypted data.
        /// </summary>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <returns></returns>
        public static string Decrypt(string encryptedData)
        {
            if (!string.IsNullOrEmpty(encryptedData))
            {
                if (ConfigurationManager.AppSettings["EncryptionPassword"] != null && ConfigurationManager.AppSettings["EncryptionSalt"] != null)
                {
                    byte[] cipherBytes = Convert.FromBase64String(encryptedData);

                    string password = ConfigurationManager.AppSettings["EncryptionPassword"];

                    byte[] salt = new ASCIIEncoding().GetBytes(ConfigurationManager.AppSettings["EncryptionSalt"]);

                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt);

                    byte[] decPwd = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
                    return Encoding.Unicode.GetString(decPwd);
                }
                throw new ConfigurationErrorsException("You need to configure the encryption password and salt in the configuration file");
            }
            throw new ArgumentNullException("encryptedData", "Data to decrypt is empty");
        }

        #endregion
    }
}