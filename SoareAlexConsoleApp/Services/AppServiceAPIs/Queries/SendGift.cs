using System.Net;
using SoareAlexConsoleApp.Services.Game;

namespace SoareAlexConsoleApp.Services.AppServiceAPIs.Queries
{
    public class SendGift
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
}
