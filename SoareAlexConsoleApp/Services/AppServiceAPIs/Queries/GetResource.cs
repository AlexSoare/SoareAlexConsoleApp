using System.Net;
using SoareAlexConsoleApp.Services.Game;

namespace SoareAlexConsoleApp.Services.AppServiceAPIs.Queries
{
    public class GetResource
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
}
