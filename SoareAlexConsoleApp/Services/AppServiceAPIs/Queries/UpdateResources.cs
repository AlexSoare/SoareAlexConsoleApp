using SoareAlexConsoleApp.Services.AppServiceAPIs.Data;
using SoareAlexConsoleApp.Services.Game;
using System.Net;

namespace SoareAlexConsoleApp.Services.AppServiceAPIs.Queries
{
    public class UpdateResources
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
}
