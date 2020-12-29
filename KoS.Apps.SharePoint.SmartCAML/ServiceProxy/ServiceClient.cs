using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace KoS.Apps.SharePoint.SmartCAML.ServiceProxy
{
    public class ServiceClient
    {
        private readonly string _serviceAddress;

        private HttpClient _client;
        private HttpClient Client => _client ?? (_client = CreateClient(_serviceAddress));

        public ServiceClient(string address)
        {
            _serviceAddress = address;
        }

        public async Task InstallationCompleted(string version)
        {
            var response = await Client.PostAsJsonAsync(Url(new[] { "trace", "InstallationCompleted" }, new 
            {
                appVersion = version,
                appCode = "smartcaml"
            }), String.Empty);

            if (!response.IsSuccessStatusCode) throw await ExceptionFromResponse(response);
        }

        public async Task UpdateCompleted(string version)
        {
            var response = await Client.PostAsJsonAsync(Url(new[] { "trace", "UpdateCompleted" }, new
            {
                appVersion = version,
                appCode = "smartcaml"
            }), String.Empty);

            if (!response.IsSuccessStatusCode) throw await ExceptionFromResponse(response);
        }

        private HttpClient CreateClient(string serviceAddress)
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip |
                                                 DecompressionMethods.Deflate;
            }

            var client = new HttpClient(handler) { BaseAddress = new Uri(serviceAddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            return client;
        }

        private static string Url(string[] parts, object urlParams = null)
        {
            return Url(parts, TypeHelper.ObjectToDictionary(urlParams));
        }

        private static string Url(string[] parts, Dictionary<string, object> urlParams = null)
        {
            var paramsString = String.Empty;

            if (urlParams != null)
            {
                foreach (var urlParam in urlParams)
                {
                    var value = urlParam.Value;

                    if (paramsString != String.Empty) paramsString += "&";

                    paramsString += $"{urlParam.Key}={value}";
                }

                if (!string.IsNullOrEmpty(paramsString)) paramsString = $"?{paramsString}";
            }

            var partsUrl = string.Join("/", parts);
            return $"{partsUrl}{paramsString}";
        }

        private async Task<Exception> ExceptionFromResponse(HttpResponseMessage response)
        {
            var error = await response.Content.ReadAsStringAsync();
            return new Exception(error);
        }
    }
}
