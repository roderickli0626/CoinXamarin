using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using mycoin.Models;
using Newtonsoft.Json;

namespace mycoin.Helpers
{
    public class HttpHelper
    {

        public EventHandler<bool> OnUnauthorizeReturned;
        private static HttpHelper _instance;
        private HttpClientHandler _handler;
        private static string _apiKey;
        string BaseUrl = "https://10.97.5.30:5001/api/";
        private HttpClient _httpClient;

        public static HttpHelper Instance => _instance ?? (_instance = new HttpHelper());


        private Dictionary<string, string> _apiVersions;
        private readonly string _defaultApiVersion = "1";
        public HttpHelper()
        {

            _handler = new HttpClientHandler { UseCookies = true };
            // Fix way for exception-SSL connection could not be established
            _handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            _httpClient = new HttpClient(_handler) { BaseAddress = new Uri(BaseUrl) };
            _apiVersions = new Dictionary<string, string>();
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

        }


        public void SetDefaultApiVersion(string url, string apiVersion = "2")
        {
            //_apiVersions[url] = apiVersion;
            //RemoveDefaultApiVersion();
            //_httpClient.DefaultRequestHeaders.Add("api-version", apiVersion);
        }


        private HttpContent createContentForApiCall(string parametersJson)
        {
            parametersJson = parametersJson ?? string.Empty;

            var bytes = Encoding.UTF8.GetBytes(parametersJson);
            var content = new ByteArrayContent(bytes);
            if (content.Headers.Contains("Content-Type"))
                content.Headers.Remove("Content-Type");
            content.Headers.Add("Content-Type", "application/json");

            return content;
        }




        public void RemoveDefaultApiVersion()
        {
            //if (_httpClient.DefaultRequestHeaders.Contains("api-version"))
            //    _httpClient.DefaultRequestHeaders.Remove("api-version");
        }

        public string GetApiVersion(string url)
        {
            return _apiVersions.ContainsKey(url) ? _apiVersions[url] : _defaultApiVersion;
        }

        public async Task<HttpResponseMessage> GetContentAsync(string url, CancellationToken token = default, bool chkapiKey = true)
        {
            //if (chkapiKey == true) await ValidateApiKey();
            HttpResponseMessage response;

            try
            {
                var callLog = new ApiCallLog();
                callLog.BaseAddress = _httpClient.BaseAddress.AbsoluteUri;
                callLog.Url = url;
                callLog.MethodType = HttpMethod.Get;
                callLog.RequestContent = null;
                callLog.DateCalled = DateTime.Now;

                response = await _httpClient.GetAsync(url, token);

                callLog.ResponseCode = response.StatusCode;
                if (response.IsSuccessStatusCode)
                    callLog.ResponseContent = null;
                else
                    callLog.ResponseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    OnUnauthorizeReturned?.Invoke(null, true);

                }
            }
            catch (HttpRequestException)
            {
                response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
            }
            catch (TimeoutException)
            {
                response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
            }
            catch (UnauthorizedAccessException ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            catch (OperationCanceledException ex)
            {
                if (token.IsCancellationRequested)
                {
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                }
                else
                {
                    response = new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
                }
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }

            _apiVersions.Remove(url);

            return response;
        }


        public async Task<HttpResponseMessage> GetContentAsyncURL(string URL, CancellationToken token = default)
        {
            //await ValidateApiKey();
            HttpResponseMessage response;

            try
            {
                HttpClient httpClient = new HttpClient();
                response = await httpClient.GetAsync(URL, token);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    OnUnauthorizeReturned?.Invoke(null, true);
                }
            }
            catch (HttpRequestException)
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            catch (TimeoutException)
            {
                response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
            }
            catch (UnauthorizedAccessException ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            catch (OperationCanceledException ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }
            return response;
        }


        public async Task<T> PostContentAsync<T>(string url, object parametersJson, CancellationToken token = default, bool checkApiKey = true) where T : class
        { 
            HttpResponseMessage response = null;

            //if (checkApiKey == true) await ValidateApiKey();
            try
            {
                string jsonString = JsonConvert.SerializeObject(parametersJson);
                Console.WriteLine("POST JSON REQUEST: " + jsonString);
                var callLog = new ApiCallLog();
                callLog.BaseAddress = _httpClient.BaseAddress.AbsoluteUri;
                callLog.Url = url;
                callLog.ApiVersion = GetApiVersion(url);
                callLog.MethodType = HttpMethod.Post;
                callLog.RequestContent = jsonString;
                callLog.DateCalled = DateTime.Now;

                var content = createContentForApiCall(jsonString);
                response = await _httpClient.PostAsync(url, content, token);

                callLog.ResponseCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                    callLog.ResponseContent = null;
                else
                    callLog.ResponseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Response API SOCIAL : " + callLog.ResponseContent);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    OnUnauthorizeReturned?.Invoke(null, true);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Excetpion:: " + ex.Message);

                response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine("Excetpion:: " + ex.Message);

                response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("Excetpion:: " + ex.Message);

                if (token.IsCancellationRequested)
                {
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                }
                else
                {
                    response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Excetpion:: " + ex.Message);
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            }

            string res = "";
            try
            {
                res = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response API SOCIAL : " + res);
            }
            catch (Exception ex)
            {

            }
            if (string.IsNullOrEmpty(res))
                return null;

            T result = JsonConvert.DeserializeObject<T>(res);

            return result;
        }


        public async Task<T> PostContentAsync<T>(string BaseURL, string url, object parametersJson, CancellationToken token = default, bool checkApiKey = true) where T : class
        {
            //if (checkApiKey == true) await ValidateApiKey();

            HttpResponseMessage response;
            try
            {
                string jsonString = JsonConvert.SerializeObject(parametersJson);
                Console.WriteLine("POST PARAMS: " + jsonString);
                var callLog = new ApiCallLog();
                callLog.BaseAddress = _httpClient.BaseAddress.AbsoluteUri;
                callLog.Url = url;
                callLog.ApiVersion = GetApiVersion(url);
                callLog.MethodType = HttpMethod.Post;
                callLog.RequestContent = jsonString;
                callLog.DateCalled = DateTime.Now;

                HttpClient Client = new HttpClient();
                Client.BaseAddress = new Uri(BaseURL);
                Client.DefaultRequestHeaders.Clear();
                foreach (var v in _httpClient.DefaultRequestHeaders)
                {
                    Client.DefaultRequestHeaders.Add(v.Key, v.Value);
                }

                var content = createContentForApiCall(jsonString);
                response = await Client.PostAsync(url, content, token);
                callLog.ResponseCode = response.StatusCode;


                if (response.IsSuccessStatusCode)
                    callLog.ResponseContent = null;
                else
                    callLog.ResponseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    OnUnauthorizeReturned?.Invoke(null, true);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Excetpion:: " + ex.Message);

                response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine("Excetpion:: " + ex.Message);

                response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("Excetpion:: " + ex.Message);

                if (token.IsCancellationRequested)
                {
                    response = new HttpResponseMessage(HttpStatusCode.Conflict);
                }
                else
                {
                    response = new HttpResponseMessage(HttpStatusCode.RequestTimeout);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Excetpion:: " + ex.Message);
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            string res = "";
            try
            {
                res = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {

            }
            if (string.IsNullOrEmpty(res))
                return null;

            T result = JsonConvert.DeserializeObject<T>(res);

            return result;
        }


        //private async Task ValidateApiKey()
        //{
        //    string APIKey = Preferences.Get(Constant.ApiKey, null);
        //    _httpClient.DefaultRequestHeaders.Remove("x-api-key");
        //    if (string.IsNullOrEmpty(APIKey))
        //    {
        //        await RefereshApiKey();
        //    }
        //    else
        //    {
        //        try
        //        {
        //            ApiKeyResponse res = JsonConvert.DeserializeObject<ApiKeyResponse>(APIKey);
        //            if (res != null)
        //            {
        //                DateTime expiredt = Convert.ToDateTime(res.expirationDT);
        //                if (expiredt > DateTime.Now) _apiKey = res.keyValue;
        //                else await RefereshApiKey();
        //            }
        //            else await RefereshApiKey();
        //        }
        //        catch (Exception)
        //        {


        //        }
        //    }
        //    _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        //}

        //public async Task<bool?> RefereshApiKey()
        //{
        //    if (Preferences.ContainsKey(Constant.userDetails))
        //    {
        //        string userDetails = Preferences.Get(Constant.userDetails, null);
        //        LoginResponse response = JsonConvert.DeserializeObject<LoginResponse>(userDetails);

        //        string IP = DependencyService.Get<IIPAddressManager>().GetIPAddress();

        //        if (string.IsNullOrEmpty(IP))
        //        {
        //            IP = Constant.StaticIP;
        //        }
        //        ApiKeyRequest req = new ApiKeyRequest
        //        {
        //            IP = IP,
        //            UserId = response.UserId.ToString()
        //        };
        //        try
        //        {
        //            ApiKeyResponse result = await PostContentAsync<ApiKeyResponse>(ApiURLs.ApiKeyURL, req, checkApiKey: false);
        //            _apiKey = result.keyValue;
        //            Preferences.Set(Constant.ApiKey, JsonConvert.SerializeObject(result));
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        return true;
        //    }
        //    return null;

        //}
    }
}
