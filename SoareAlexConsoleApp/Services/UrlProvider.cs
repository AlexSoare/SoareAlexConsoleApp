namespace SoareAlexConsoleApp.Services
{
    public enum EnvironmentType
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

        private EnvironmentType currentUrlType;

        public string BaseUrl
        {
            get
            {
                switch (currentUrlType)
                {
                    case EnvironmentType.Development: return baseURL_Local;
                    case EnvironmentType.Production: return baseURL_Web;
                    case EnvironmentType.Custom: return baseURL_Custom;
                    default: return baseURL_Local;
                }
            }
        }

        public string WebSocketBaseUrl
        {
            get
            {
                switch (currentUrlType)
                {
                    case EnvironmentType.Development: return webSocketBaseURL_Local;
                    case EnvironmentType.Production: return webSocketBaseURL_Web;
                    case EnvironmentType.Custom: return webSocketBaseURLL_Custom;
                    default: return webSocketBaseURL_Local;
                }
            }
        }

        public void SetUrlType(EnvironmentType type)
        {
            currentUrlType = type;
        }

        public void SetCustomUrl(string baseUrl, string webSocketUrl)
        {
            if (baseUrl.Last() == '/' || baseUrl.Last() == '\\')
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

            if (webSocketUrl.Last() == '/' || webSocketUrl.Last() == '\\')
                webSocketUrl = webSocketUrl.Substring(0, webSocketUrl.Length - 1);

            baseURL_Custom = baseUrl;
            webSocketBaseURLL_Custom = webSocketUrl;
            currentUrlType = EnvironmentType.Custom;
        }
    }
}
