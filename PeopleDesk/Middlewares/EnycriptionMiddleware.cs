using System.Security.Cryptography;
using System.Text;

namespace PeopleDesk.Middlewares
{
    public class EnycriptionMiddleware
    {
        private readonly RequestDelegate _next;

        public EnycriptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!httpContext.Request.Path.ToString().ToUpper().Contains("RDLCPDF")
                   && !httpContext.Request.Path.ToString().ToUpper().Contains("RDLCHTML")
                   && !httpContext.Request.Path.ToString().ToUpper().Contains("RDLCXLSX"))
            {
                httpContext.Response.Body = EncryptStream(httpContext.Response.Body);
            }
            httpContext.Request.Body = DecryptStream(httpContext.Request.Body);
            if (httpContext.Request.QueryString.HasValue)
            {
                string decryptedString = DecryptString(httpContext.Request.QueryString.Value.Substring(1));
                httpContext.Request.QueryString = new QueryString($"?{decryptedString}");
            }
            await _next(httpContext);
            await httpContext.Request.Body.DisposeAsync();
            await httpContext.Response.Body.DisposeAsync();
        }

        private static CryptoStream EncryptStream(Stream responseStream)
        {
            Aes aes = GetEncryptionAlgorithm();

            ToBase64Transform base64Transform = new ToBase64Transform();
            CryptoStream base64EncodedStream = new CryptoStream(responseStream, base64Transform, CryptoStreamMode.Write);
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            CryptoStream cryptoStream = new CryptoStream(base64EncodedStream, encryptor, CryptoStreamMode.Write);

            return cryptoStream;
        }

        private static Stream DecryptStream(Stream cipherStream)
        {
            Aes aes = GetEncryptionAlgorithm();

            FromBase64Transform base64Transform = new FromBase64Transform(FromBase64TransformMode.IgnoreWhiteSpaces);
            CryptoStream base64DecodedStream = new CryptoStream(cipherStream, base64Transform, CryptoStreamMode.Read);
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            CryptoStream decryptedStream = new CryptoStream(base64DecodedStream, decryptor, CryptoStreamMode.Read);
            return decryptedStream;
        }

        private static Aes GetEncryptionAlgorithm()
        {
            Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes("@#%FWFJW#R(%adfs");
            aes.IV = Encoding.UTF8.GetBytes("@#%FWFJW#R(%adfs");

            return aes;
        }

        private static string DecryptString(string cipherText)
        {
            Aes aes = GetEncryptionAlgorithm();
            byte[] buffer = Convert.FromBase64String(cipherText);

            using MemoryStream memoryStream = new MemoryStream(buffer);
            using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new StreamReader(cryptoStream);
            return streamReader.ReadToEnd();
        }
    }
}