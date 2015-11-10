using SafeBrowsingLookup;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Framework.Configuration;
using Microsoft.Dnx.Runtime;

namespace Sample
{
    public class Program
    {
        private LookupClient client;
        private IConfiguration Configuration { get; set; }
        public Program(IApplicationEnvironment appEnv)
        {
            Initialize(appEnv);
        }
        private void Initialize(IApplicationEnvironment appEnv)
        {
            string jsonFile;
#if DEBUG
            jsonFile = "Config.local.json";
#else
            jsonFile = "Config.json";
#endif
            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile(jsonFile);
            Configuration = builder.Build();
            client = new LookupClient(Configuration["Data:ApiKey"], Configuration["Data:ClientName"]);
        }
        public void Main(string[] args)
        {
            Console.WriteLine("====== Safe Browsing API - .NET Sample ======");

            // This queries Google API for one URL
            HandleUrl("alarash.net");

            // This queries Google API for multiple URLs
            HandleUrls();
            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
        }
        public void HandleUrl(string url)
        {
            Console.Write("Looking up `{0}` : ", url);
            ResponseType zob = Lookup(url).Result;
            string consoleMessage = "";
            switch (zob)
            {
                case ResponseType.OK:
                    consoleMessage = "URL appears to be safe.";
                    break;

                case ResponseType.MALWARE:
                    consoleMessage = "URL is suspected of hosting malware.";
                    break;

                case ResponseType.MALWARE_UNWANTED:
                    consoleMessage = "URL is suspected of hosting malware and unwanted software.";
                    break;

                case ResponseType.PHISHING:
                    consoleMessage = "URL is suspected of phishing.";
                    break;

                case ResponseType.PHISHING_MALWARE:
                    consoleMessage = "URL is suspected of phishing and hosting malware.";
                    break;

                case ResponseType.PHISHING_MALWARE_UNWANTED:
                    consoleMessage = "URL is suspected of phishing and hosting malware and unwanted software.";
                    break;

                case ResponseType.PHISHING_UNWANTED:
                    consoleMessage = "URL is suspected of phishing and hosting unwanted software.";
                    break;

                case ResponseType.UNWANTED:
                    consoleMessage = "URL is suspected of hosting unwanted software.";
                    break;

            }
            Console.WriteLine(consoleMessage);
        }
        public void HandleUrls()
        {
            IList<string> urls = new List<string>();
            urls.Add("http://alarash.net"); // OK
            urls.Add("http://gumblar.cn"); // MALWARE
            urls.Add("http://microsoft.com"); // OK
            urls.Add("http://google.com"); // OK
            urls.Add("http://apple.com"); // OK
            urls.Add("http://reddit.com"); // OK
            urls.Add("http://amazon.com"); // OK
            urls.Add("http://github.com"); // OK
            IDictionary<string, ResponseType> result = client.Lookup(urls).Result;
            foreach(KeyValuePair<string, ResponseType> entry in result)
            {
                Console.WriteLine("{0}: {1}", entry.Key, entry.Value.ToString());
            }
        }
        public async Task<ResponseType> Lookup(string url)
        {
            return await client.Lookup(url);
        }
    }
}
