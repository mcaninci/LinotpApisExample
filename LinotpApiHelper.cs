using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;


namespace LinotpTokenDelete
{
    public static class LinotpApiHelper
    {
        #region "LinOTP Token Find And Delete For User"
        public static Tuple<TokenModel, string> SyncDeleteToken(string userName)
        {
            return AsyncHelper.RunSync<Tuple<TokenModel, string>>(() => DeleteToken(userName));
        }
        public static async Task<Tuple<TokenModel, string>> DeleteToken(string userName)
        {
            System.Net.Security.RemoteCertificateValidationCallback ignoreSSLErrors = (sender, certificate, chain, sslPolicyErrors) => true;
            System.Net.ServicePointManager.ServerCertificateValidationCallback += ignoreSSLErrors;

            var response = await AdminGetSession();
            if (response.Item1 == null && !string.IsNullOrEmpty(response.Item2))
                return response;

            string session = response.Item1.session;
            Dictionary<string, string> headers = response.Item1.headers;

            var existTokenRes = await AdminShowToken(session, headers, userName);
            if (existTokenRes.Item1 == null && !string.IsNullOrEmpty(existTokenRes.Item2))
                return new Tuple<TokenModel, string>(null, existTokenRes.Item2);


            var deleteTokenRes = await AdminDeleteToken(session, headers, existTokenRes.Item1.LinOtpTokenSerialnumber);
            if (deleteTokenRes.Item1 == null && !string.IsNullOrEmpty(deleteTokenRes.Item2))
                return deleteTokenRes;



            return new Tuple<TokenModel, string>(null, "Token number " + existTokenRes.Item1.LinOtpTokenSerialnumber + " has been deleted for user" + userName);
        }

        public static async Task<Tuple<TokenModel, string>> AdminGetSession()
        {
            string URI = Settings.LinOTPAddress + "/admin/getsession";

            try
            {
                var response = await AsyncPostApi<TokenModel>(URI, null, null);

                TokenModel tokenModel = response.Item1;

                if (tokenModel.result.status && tokenModel.result.value)
                {

                    List<Cookie> responseCookies = response.Item3;
                    var sessionCookie = responseCookies.Where(x => x.Name == "admin_session").FirstOrDefault();

                    if (sessionCookie == null || string.IsNullOrEmpty(sessionCookie.Value))
                    {
                        return new Tuple<TokenModel, string>(null, "User authentication failed.please try again end mail");
                    }

                    return new Tuple<TokenModel, string>(new TokenModel() { session = sessionCookie.Value, headers = response.Item2 }, "");
                }
                else
                {
                    return new Tuple<TokenModel, string>(null, "User authentication failed.please try again end mail");
                }


            }
            catch (Exception ex)
            {
                return new Tuple<TokenModel, string>(null, "User authentication failed.please try again end mail. Error:" + ex.Message);
            }


        }

        public static async Task<Tuple<TokenDetail, string>> AdminShowToken(string session, Dictionary<string, string> headers, string userName)
        {

            string URI = Settings.LinOTPAddress + "/admin/show";

            try
            {
                headers.Add("admin_session", session);

                var content = new FormUrlEncodedContent(new[]
                  {
                        new KeyValuePair<string, string>("session", session)
                    });

                var response = await AsyncPostApi<TokenListModel>(URI, content, headers);


                var deserializedtoken = response.Item1;

                if (deserializedtoken.result.status == false)
                {
                    return new Tuple<TokenDetail, string>(null, "Token list not fetched");
                }
                else
                {

                    var currentUserTokenInfo = deserializedtoken.result.value.data.Where(x => x.UserUsername == userName && x.LinOtpIsactive == true).FirstOrDefault();
                    if (currentUserTokenInfo == null)
                        return new Tuple<TokenDetail, string>(null, "The user doesn't have an active token.");

                    return new Tuple<TokenDetail, string>(currentUserTokenInfo, "");
                }
            }
            catch (Exception ex)
            {
                return new Tuple<TokenDetail, string>(null, "Get show all token operation failed. Error: " + ex.Message);
            }

        }


        public static async Task<Tuple<TokenModel, string>> AdminDeleteToken(string session, Dictionary<string, string> headers, string sessionSerial)
        {
            string URI = Settings.LinOTPAddress + "/admin/remove";

            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("session", session),
                        new KeyValuePair<string, string>("serial", sessionSerial)
                 });

                var response = await AsyncPostApi<TokenModel>(URI, content, headers);

                var deserializedtoken = response.Item1;

                if (deserializedtoken.result.status == false)
                {
                    return new Tuple<TokenModel, string>(null, "Delete token operation failed.");
                }
                else
                {
                    return new Tuple<TokenModel, string>(null, "");
                }
            }
            catch (Exception ex)
            {
                return new Tuple<TokenModel, string>(null, "Get show all token operation failed. Error: " + ex.Message);
            }
        }

        public static async Task<Tuple<T, Dictionary<string, string>, List<Cookie>>> AsyncPostApi<T>(string url, FormUrlEncodedContent content, Dictionary<string, string> headers)
        {
            T responseModel = default(T);
            CookieContainer cookieContainer = new CookieContainer();
            var baseAddress = new Uri(Settings.LinOTPAddress);
            HttpClientHandler handler = new HttpClientHandler();

            Dictionary<string, string> headersOutgoing = new Dictionary<string, string>();
            handler.CookieContainer = cookieContainer;
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };


            var credentials = new NetworkCredential(Settings.User, Settings.Password);
            handler.Credentials = credentials;

            using (HttpClient client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                client.DefaultRequestHeaders.Clear();
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                    cookieContainer.Add(baseAddress, new Cookie("admin_session", headers["admin_session"]));
                }

                var response = await client.PostAsync(url, content).ConfigureAwait(false);
                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

              
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("Api connection issue");

                foreach (var header in response.Headers)
                {
                    headersOutgoing.Add(header.Key, header.Value.First());
                }

                Uri uri = new Uri(url);
                List<Cookie> responseCookies = cookieContainer.GetCookies(uri).Cast<Cookie>().ToList();

                responseModel = JsonConvert.DeserializeObject<T>(data);



                return new Tuple<T, Dictionary<string, string>, List<Cookie>>(responseModel, headersOutgoing, responseCookies);
            }
        }
        #endregion
    }



}
