using MZBlog.Web.Features;
using Nancy;
using Nancy.Cookies;
using Nancy.Cryptography;
using System;

namespace MZBlog.Web.Security
{
    public class FormsAuthentication
    {
        private static readonly IHmacProvider hmacProvider =
                new DefaultHmacProvider(
                    new PassphraseKeyGenerator(
                        AppConfiguration.Current.EncryptionKey,
                        new byte[] { 8, 2, 10, 4, 68, 120, 7, 14 }
                    )
                );

        private static readonly IEncryptionProvider encryptionProvider =
            new RijndaelEncryptionProvider(
                new PassphraseKeyGenerator(
                    AppConfiguration.Current.HmacKey,
                    new byte[] { 1, 20, 73, 49, 25, 106, 78, 86 }
                )
            );

        private static readonly string FormsAuthenticationCookie = "_auth";

        public static string DecryptAndValidateAuthenticationCookie(string cookieValue)
        {
            var hmacLength = Base64Helpers.GetBase64Length(hmacProvider.HmacLength);

            var hmacValue = cookieValue.Substring(0, hmacLength);
            var encryptCookie = cookieValue.Substring(hmacLength);

            // Check the hmac, but don't early exit if they don't match
            var bytes = Convert.FromBase64String(hmacValue);
            var newHmac = hmacProvider.GenerateHmac(encryptCookie);
            var hmacValid = HmacComparer.Compare(newHmac, bytes, hmacProvider.HmacLength);

            var decrypted = encryptionProvider.Decrypt(encryptCookie);

            // Only return the decrypted result if tht hmac was ok
            return hmacValid ? decrypted : string.Empty;
        }

        public static string EncryptAndSignCookie(string cookieValue)
        {
            var encryptCookie = encryptionProvider.Encrypt(cookieValue);

            var hmacBytes = hmacProvider.GenerateHmac(encryptCookie);
            var hmacValue = Convert.ToBase64String(hmacBytes);

            return string.Format("{0}{1}", hmacValue, encryptCookie);
        }

        public static NancyCookie CreateAuthCookie(string username)
        {
            var encryptedCookieValue = EncryptAndSignCookie(username);

            return new NancyCookie(FormsAuthenticationCookie, encryptedCookieValue, true, false);
        }

        public static NancyCookie CreateLogoutCookie()
        {
            return new NancyCookie(FormsAuthenticationCookie, "", true, false)
            {
                Expires = DateTime.UtcNow.AddDays(-1)
            };
        }

        public static string GetAuthUsernameFromCookie(NancyContext ctx)
        {
            if (!ctx.Request.Cookies.ContainsKey(FormsAuthenticationCookie))
            {
                return string.Empty;
            }

            var cookie = ctx.Request.Cookies[FormsAuthenticationCookie];
            var usernameCookie = DecryptAndValidateAuthenticationCookie(cookie);
            return usernameCookie;
        }
    }
}