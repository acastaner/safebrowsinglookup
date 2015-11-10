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
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Safe Browsing API .NET Client/" + ClientVersion);
            SetupRequestUrl();
        }
        private void SetupRequestUrl()
        {
            using (var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("client", ClientName),
                new KeyValuePair<string, string>("key", ApiKey),
                new KeyValuePair<string, string>("appver", ClientVersion),
                new KeyValuePair<string, string>("pver", ProtocolVersion)
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
            string requestUrl = RequestUrl + "&url=" + WebUtility.UrlEncode(url);
            HttpResponseMessage response = await HttpClient.GetAsync(requestUrl);
            // If URL is safe Google returns a 204 status code so we can just break here if that's the case
            if ((int)response.StatusCode == 204)
                return ResponseType.OK;

            var content = await response.Content.ReadAsStringAsync();
            return await CategorizeContent(content);
        }
        public async Task<IDictionary<string, ResponseType>> Lookup(IList<string> urls)
        {
            string content = urls.Count.ToString();
            foreach (string url in urls)
                content += "\n" + url;

            StringContent sc = new StringContent(content);
            HttpResponseMessage response = await HttpClient.PostAsync(RequestUrl, new StringContent(content));
            string responseContent = await response.Content.ReadAsStringAsync();
            return await SplitResponseTypes(responseContent, urls);
        }
        public async Task<ResponseType> CategorizeContent(string content)
        {
            switch (content)
            {
                case "ok":
                    return ResponseType.OK;
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
        public async Task<IDictionary<string, ResponseType>> SplitResponseTypes(string content, IList<string> urls)
        {
            string[] lines = content.Split('\n');
            IDictionary<string, ResponseType> result = new Dictionary<string, ResponseType>();
            for(int i = 0; i <= lines.Count() - 1; i++)
            {
                ResponseType responseType = await CategorizeContent(lines[i]);
                result.Add(urls[i], responseType);
            }
            return result;
        }
    }
}
