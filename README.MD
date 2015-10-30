Safe Browsing Lookup .NET API
=========

This library allows to easily query Google's Safe Browsing Lookup API from .NET. It's a library package compatible with .NET 4.5 and ASP.NET 5 (CoreCLR and otherwise). Please note that this library doesn't offer any guarantee of protection - a website deemed safe could have malware, or a website can be incorrectly flagged as hosting unwated software. Google works to provide the most accurate and up-to-date phishing, malware, and unwanted software information. However, Google cannot guarantee that its information is comprehensive and error-free: some risky sites may not be identified, and some safe sites may be identified in error. 

This project and its author are not affiliated to Google in any way.

# Sample 
 
 The source code comes with a sample Console application but here are some more code samples:
 
 ```csharp
LookupClient client = new LookupClient("someApiKey", "someClientName");
 ```
 
 This code will instantiate a LookupClient object. You need to provide two parameters: your Google API Key and a client name. The client name can be anything, although Google suggests you use a company name or something similar.
 
 Then it's only a matter of sending the lookup request:
 
 ```csharp
ResponseType response = client.Lookup(url);
 ```
 
 The ResponseType is an enumerator which contain the type of response as defined by Google. Only one extra type was added, "UNKNOWN", for error handling in this library. To check the response type, something like this can be done:
 
 ```csharp
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
 ```
