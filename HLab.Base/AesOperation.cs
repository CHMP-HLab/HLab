using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HLab.Base;

public static class AesOperation
{
    static readonly byte[] Salt = { 124, 243, 27 , 121, 98, 45, 76, 3};

    static byte[] CreateKey(string password, int keyBytes, int iterations)
    {
        var keyGenerator = new Rfc2898DeriveBytes(password, Salt, iterations);
        return keyGenerator.GetBytes(keyBytes);
    }

    public static string Encrypt(string key, string plainText)
    {
        using var aes = Aes.Create();

        aes.Key = CreateKey(key,32,321);
        aes.IV = CreateKey(key,16,277);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var stream = new MemoryStream();
        using (var streamWriter = new StreamWriter(
                   new CryptoStream(stream, 
                       aes.CreateEncryptor(aes.Key, aes.IV), 
                       CryptoStreamMode.Write
                   )
               ))
        {
            streamWriter.Write(plainText);
        }

        return  Convert.ToBase64String(stream.ToArray());
    }

    public static string Decrypt(string key, string cipherText)
    {
        var buffer = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = CreateKey(key,32,321);
        aes.IV = CreateKey(key,16,277);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var memoryStream = new MemoryStream(buffer);

        using var stream = new StreamReader
        (
            new CryptoStream(memoryStream,
                aes.CreateDecryptor(aes.Key, aes.IV),
                CryptoStreamMode.Read
            )
        );
        return stream.ReadToEnd();
    }
}