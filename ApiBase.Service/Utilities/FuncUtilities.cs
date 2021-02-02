using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IO;

namespace ApiBase.Service.Utilities
{
    public class FuncUtilities
    {
        public static DateTime GetDateCurrent()
        {
            string date = DateTime.Now.ToString("dd/MM/yyyy");
            DateTime d = new DateTime();
            if (date != "")
            {
                d = DateTime.ParseExact(date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                d = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            return d;
        }
        public static DateTime GetDateTimeCurrent()
        {
            string date = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
            DateTime d = new DateTime();
            if (date != "")
            {
                d = DateTime.ParseExact(date, "dd/MM/yyyy hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                d = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), "dd/MM/yyyy hh:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            return d;
        }
        public static DateTime ConvertStringToDateTime(string date = "")
        {
            DateTime d = new DateTime();
            if (date.Split('-').Count() > 1)
            {
                if (!string.IsNullOrEmpty(date))
                {
                    d = DateTime.ParseExact(date, "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    d = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                }
                return d;
            }
            if (!string.IsNullOrEmpty(date))
            {
                d = DateTime.ParseExact(date, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                d = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            return d;
        }
        public static DateTime ConvertStringToDate(string date = "")
        {
            DateTime d = new DateTime();
            if (date.Split('-').Count() > 1)
            {
                if (!string.IsNullOrEmpty(date))
                {
                    d = DateTime.ParseExact(date, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    d = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                return d;
            }
            if (!string.IsNullOrEmpty(date))
            {
                d = DateTime.ParseExact(date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                d = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            return d;
        }
        public static string ConvertDateToString(DateTime date)
        {
            string dateString = "";
            if (date != null)
                dateString = date.ToString();
            return dateString;
        }
        public static DateTime ConvertToTimeStamp(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public static string BestLower(string input = "")
        {
            if (input == null)
            {
                return "";
            }
            input = input.Trim();
            for (int i = 0x20; i < 0x30; i++)
            {
                input = input.Replace(((char)i).ToString(), " ");
            }
            input = input.Replace(".", "-");
            input = input.Replace(" ", "-");
            input = input.Replace(",", "-");
            input = input.Replace(";", "-");
            input = input.Replace(":", "-");
            input = input.Replace("  ", "-");
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string str = input.Normalize(NormalizationForm.FormD);
            string str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
            while (str2.IndexOf("?") >= 0)
            {
                str2 = str2.Remove(str2.IndexOf("?"), 1);
            }
            while (str2.Contains("--"))
            {
                str2 = str2.Replace("--", "-").ToLower();
            }
            return str2.ToLower();
        }

        public static bool ReCaptchaPassed(string secret, string gRecaptchaResponse)
        {
            HttpClient httpClient = new HttpClient();
            var res = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={gRecaptchaResponse}").Result;
            if (res.StatusCode != HttpStatusCode.OK)
                return false;

            string JSONres = res.Content.ReadAsStringAsync().Result;
            dynamic JSONdata = JObject.Parse(JSONres);
            if (JSONdata.success != "true")
                return false;

            return true;
        }

        public static int TinhKhoangCachNgay(DateTime date)
        {
            DateTime dateNow = GetDateCurrent();
            TimeSpan ts = date - dateNow;
            return ts.Days;
        }

        public static string parseJWTToEmail(string tokenstring)
        {

            tokenstring = tokenstring.Replace("Bearer ", "");
            //var stream = Encoding.ASCII.GetBytes("CYBERSOFT2020_SECRET");
            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring);
            var email = token.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            return email;
        }
        public static string Base64Encode(string plainText)
        {
            return Cipher.Encrypt(plainText.Trim(), "khai");
        }
        public static string Base64Decode(string base64EncodedData)
        {
            return Cipher.Decrypt(base64EncodedData, "khai");
        }
    }
}

public static class Cipher
{
    /// <summary>
    /// Encrypt a string.
    /// </summary>
    /// <param name="plainText">String to be encrypted</param>
    /// <param name="password">Password</param>
    public static string Encrypt(string plainText, string password)
    {
        if (plainText == null)
        {
            return null;
        }

        if (password == null)
        {
            password = String.Empty;
        }

        // Get the bytes of the string
        var bytesToBeEncrypted = Encoding.UTF8.GetBytes(plainText);
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        // Hash the password with SHA256
        passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

        var bytesEncrypted = Cipher.Encrypt(bytesToBeEncrypted, passwordBytes);

        return Convert.ToBase64String(bytesEncrypted);
    }

    /// <summary>
    /// Decrypt a string.
    /// </summary>
    /// <param name="encryptedText">String to be decrypted</param>
    /// <param name="password">Password used during encryption</param>
    /// <exception cref="FormatException"></exception>
    public static string Decrypt(string encryptedText, string password)
    {
        if (encryptedText == null)
        {
            return null;
        }

        if (password == null)
        {
            password = String.Empty;
        }

        // Get the bytes of the string
        var bytesToBeDecrypted = Convert.FromBase64String(encryptedText);
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

        var bytesDecrypted = Cipher.Decrypt(bytesToBeDecrypted, passwordBytes);
        string kq = Encoding.UTF8.GetString(bytesDecrypted);
        return kq;
    }

    private static byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
    {
        byte[] encryptedBytes = null;

        // Set your salt here, change it to meet your flavor:
        // The salt bytes must be at least 8 bytes.
        var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        using (MemoryStream ms = new MemoryStream())
        {
            using (RijndaelManaged AES = new RijndaelManaged())
            {
                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                AES.KeySize = 256;
                AES.BlockSize = 128;
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    cs.Close();
                }

                encryptedBytes = ms.ToArray();
            }
        }

        return encryptedBytes;
    }

    private static byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
    {
        byte[] decryptedBytes = null;

        // Set your salt here, change it to meet your flavor:
        // The salt bytes must be at least 8 bytes.
        var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        using (MemoryStream ms = new MemoryStream())
        {
            using (RijndaelManaged AES = new RijndaelManaged())
            {
                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                AES.KeySize = 256;
                AES.BlockSize = 128;
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);
                AES.Mode = CipherMode.CBC;

                using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                    cs.Close();
                }

                decryptedBytes = ms.ToArray();
            }
        }

        return decryptedBytes;
    }
}