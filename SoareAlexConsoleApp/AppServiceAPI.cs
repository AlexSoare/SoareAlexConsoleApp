using Microsoft.Extensions.Logging;
using System.Data.SqlTypes;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;

namespace SoareAlexConsoleApp
{
    public class LoginQuery
    {
        public class QueryRequest
        {
            public string DeviceId { get; set; }
        }

        public class QueryResponse
        {
            public string PlayerId { get; set; }
            public bool AlreadyOnline { get; set; }
            public string AuthToken { get; set; }
            public HttpStatusCode Status { get; set; }
        }
    }
    public class SendGiftQuery
    {
        public class QueryRequest
        {
            public string FriendPlayerId { get; set; }
            public ResourceType ResourceType { get; set; }
            public double ResourceValue { get; set; }
        }

        public class QueryResponse
        {
            public HttpStatusCode Status { get; set; }
        }
    }
    public class GetResourceQuery
    {
        public class QueryRequest
        {
            public ResourceType ResourceType { get; set; }
        }

        public class QueryResponse
        {
            public double ResourceValue { get; set; }
            public HttpStatusCode Status { get; set; }
        }
    }
    public class UpdateResourcesQuery
    {
        public class QueryRequest
        {
            public ResourceType ResourceType { get; set; }
            public double ResourceValue { get; set; }
        }

        public class QueryResponse
        {
            public List<Resource> UpdatedResources { get; set; }
            public HttpStatusCode Status { get; set; }
        }
    }
    public class Resource
    {
        public ResourceType ResourceType { get; set; }
        public double Value { get; set; }
    }

    public class AppServiceAPI
    {
        private readonly ILogger<AppServiceAPI> logger;
        private readonly UrlProvider urlProvider;

        private string authToken;

        public AppServiceAPI(ILogger<AppServiceAPI> logger, UrlProvider urlProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.urlProvider = urlProvider ?? throw new ArgumentNullException(nameof(urlProvider));
        }

        public void SetCurrentAuthToken(string authToken)
        {
            this.authToken = authToken;
        }

        public async Task PostRequest<T>(string api, Object requestBody, Action<T> responseCallback) where T: class
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var url = urlProvider.BaseUrl + api;

                    var json = JsonSerializer.Serialize(requestBody);

                    byte[] bytes = Encoding.ASCII.GetBytes(json);

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    if (!string.IsNullOrEmpty(authToken))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                    HttpResponseMessage response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            var responseObj = JsonSerializer.Deserialize<T>(responseContent);
                            responseCallback(responseObj);
                        }
                        else
                        {
                            logger.LogError($"Empty response received!");
                            responseCallback(null);
                        }
                    }
                    else
                    {
                        logger.LogError($"Failed to get data. Status code: {response.StatusCode}");
                        responseCallback(null);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                logger.LogError($"An error occurred");
                logger.LogError(ex.Message);
                responseCallback(null);
            }
        }
        public async Task GetRequest<T>(string api, Object requestBody, Action<T> responseCallback) where T : class
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var url = urlProvider.BaseUrl + api;

                    var queryString = HttpUtility.ParseQueryString("");
                    foreach (var property in requestBody.GetType().GetProperties())
                    {
                        var value = property.GetValue(requestBody);
                        if (value != null)
                        {
                            queryString[property.Name] = value.ToString();
                        }

                        url += "?" + queryString.ToString();
                    }

                    if (!string.IsNullOrEmpty(authToken))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                    HttpResponseMessage response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            var responseObj = JsonSerializer.Deserialize<T>(responseContent);
                            responseCallback(responseObj);
                        }
                        else
                        {
                            logger.LogError($"Empty response received!");

                        }
                    }
                    else
                    {
                        logger.LogError($"Failed to get data. Status code: {response.StatusCode}");
                        responseCallback(null);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                logger.LogError($"An error occurred");
                logger.LogError(ex.Message);
                responseCallback(null);
            }
        }
    }
}
