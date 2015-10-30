using SafeBrowsingLookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample
{
    public class Program
    {
        private LookupClient client;
        public void Main(string[] args)
        {
            Console.WriteLine("====== Safe Browsing API - .NET Sample ======");
            client = new LookupClient("AIzaSyAt8vyn6EcbmGzHyfhNhTdfqGn2z8RYRsY", "dotnet-sample-client");
            HandleUrl("gumblar.cn");
            HandleUrl("alarash.net");
            HandleUrl("jbleon.org");
            HandleUrl("miss-diamond.dj");
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
        public async Task<ResponseType> Lookup(string url)
        {
            return await client.Lookup(url);
        }
    }
}
