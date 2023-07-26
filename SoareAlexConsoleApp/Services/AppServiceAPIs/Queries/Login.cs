using System.Net;

namespace SoareAlexConsoleApp.Services.AppServiceAPIs.Queries
{
    public class Login
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
}
