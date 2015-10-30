using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SafeBrowsingLookup
{
    public class LookupClient
    {
        private string ApiKey { get; set; }
        private string ClientName { get; set; }
        private string ClientVersion { get; set; }
        private string ProtocolVersion { get; set; }
        private HttpClientHandler HttpClientHandler { get; set; }
        private HttpClient HttpClient { get; set; }
        private string RequestUrl { get; set; }
        public LookupClient(string apiKey, string clientName)
        {
            ApiKey = apiKey;
            ClientName = clientName;
            ProtocolVersion = "3.1";
            ClientVersion = "0.1";

            // Setup HTTP Client for requests
            HttpClientHandler = new HttpClientHandler();
            HttpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            HttpClientHandler.Credentials = CredentialCache.DefaultCredentials;
            //HttpClientHandler.Proxy = WebRequest.DefaultWebProxy;
            //HttpClientHandler.Proxy.Credentials = CredentialCache.DefaultCredentials;

            HttpClient = new HttpClient(HttpClientHandler);
            HttpClient.BaseAddress = new Uri("https://sb-ssl.google.com/safebrowsing/api/lookup?");
            //HttpClient.DefaultRequestHeaders.Accept.Clear();
            //HttpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            //HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Safe Browsing API .NET Client/0.1");
        }
        private void SetRequestUrl(string lookupUrl)
        {
            using (var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("client", ClientName),
                new KeyValuePair<string, string>("key", ApiKey),
                new KeyValuePair<string, string>("appver", ClientVersion),
                new KeyValuePair<string, string>("pver", ProtocolVersion),
                new KeyValuePair<string, string>("url", WebUtility.UrlEncode(lookupUrl)),
            }
            ))
                {
                RequestUrl = HttpClient.BaseAddress.ToString() + content.ReadAsStringAsync().Result;
                }
        }
        public void SetApiKey(string apiKey)
        {
            ApiKey = apiKey;
        }
        public void SetClientName(string clientName)
        {
            ClientName = clientName;
        }
        public async Task<ResponseType> Lookup(string url)
        {
            SetRequestUrl(url);
            HttpResponseMessage response = await HttpClient.GetAsync(RequestUrl);
            // If URL is safe Google returns a 204 status code so we can just break here if that's the case
            if ((int)response.StatusCode == 204)
                return ResponseType.OK;

            var content = await response.Content.ReadAsStringAsync();
            return await CategorizeContent(content);
        }
        public async Task<ResponseType> CategorizeContent(string content)
        {
            switch (content)
            {
                case "phishing":
                    return ResponseType.PHISHING;
                case "malware":
                    return ResponseType.MALWARE;
                case "unwanted":
                    return ResponseType.UNWANTED;
                case "phishing,malware":
                    return ResponseType.PHISHING_MALWARE;
                case "phishing,unwanted":
                    return ResponseType.PHISHING_UNWANTED;
                case "malware,unwanted":
                    return ResponseType.MALWARE_UNWANTED;
                case "phishing,malware,unwanted":
                    return ResponseType.PHISHING_MALWARE_UNWANTED;
                default:
                    return ResponseType.UNKNOWN;
            }
        }
    }
}
