using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoareAlexConsoleApp
{
    public enum EnviromentType
    {
        Development,
        Production,
        Custom
    }

    public class UrlProvider
    {
        private const string baseURL_Local = "https://localhost:7131";
        private const string webSocketBaseURL_Local = "wss://localhost:7131";

        private const string baseURL_Web = "https://soarenitagameserver.azurewebsites.net";
        private const string webSocketBaseURL_Web = "wss://soarenitagameserver.azurewebsites.net";

        private string baseURL_Custom = "";
        private string webSocketBaseURLL_Custom = "";

        private EnviromentType currentUrlType;

        public string BaseUrl { get
            {
                switch(currentUrlType)
                {
                    case EnviromentType.Development: return baseURL_Local;
                    case EnviromentType.Production: return baseURL_Web;
                    case EnviromentType.Custom: return baseURL_Custom;
                    default: return baseURL_Local;
                }
            } }

        public string WebSocketBaseUrl
        {
            get
            {
                switch (currentUrlType)
                {
                    case EnviromentType.Development: return webSocketBaseURL_Local;
                    case EnviromentType.Production: return webSocketBaseURL_Web;
                    case EnviromentType.Custom: return webSocketBaseURLL_Custom;
                    default: return webSocketBaseURL_Local;
                }
            }
        }

        public void SetUrlType(EnviromentType type)
        {
            currentUrlType = type;
        }

        public void SetCustomUrl(string baseUrl, string webSocketUrl)
        {
            if(baseUrl.Last() == '/' || baseUrl.Last() == '\\')
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

            if (webSocketUrl.Last() == '/' || webSocketUrl.Last() == '\\')
                webSocketUrl = webSocketUrl.Substring(0, webSocketUrl.Length - 1);

            baseURL_Custom = baseUrl;
            webSocketBaseURLL_Custom = webSocketUrl;
            currentUrlType = EnviromentType.Custom;
        }
    }
}
