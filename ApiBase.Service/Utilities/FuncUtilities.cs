﻿using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}