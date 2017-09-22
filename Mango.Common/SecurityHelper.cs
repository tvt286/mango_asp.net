using System;
using System.Configuration;

namespace Kootoro.Common
{
    public static class SecurityHelper
    {
        public static string Encrypt(string original)
        {
            EncryptionClassLibrary.Encryption.Symmetric sym = new EncryptionClassLibrary.Encryption.Symmetric(EncryptionClassLibrary.Encryption.Symmetric.Provider.DES);
            EncryptionClassLibrary.Encryption.Data key = new EncryptionClassLibrary.Encryption.Data(ConfigurationManager.AppSettings["EncryptKey"]);
            EncryptionClassLibrary.Encryption.Data encryptedData = sym.Encrypt(new EncryptionClassLibrary.Encryption.Data(original), key);
            return encryptedData.Hex;
        }

        public static string Decrypt(string encryptedString)
        {
            try
            {
                EncryptionClassLibrary.Encryption.Symmetric sym = new EncryptionClassLibrary.Encryption.Symmetric(EncryptionClassLibrary.Encryption.Symmetric.Provider.DES);
                EncryptionClassLibrary.Encryption.Data key = new EncryptionClassLibrary.Encryption.Data(ConfigurationManager.AppSettings["EncryptKey"]);
                EncryptionClassLibrary.Encryption.Data encryptedData = new EncryptionClassLibrary.Encryption.Data();
                encryptedData.Hex = encryptedString;
                EncryptionClassLibrary.Encryption.Data decryptedData = sym.Decrypt(encryptedData, key);
                return decryptedData.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
