using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace SoareAlexConsoleApp
{
    public enum WebSocketEvent
    {
        GiftEvent
    }
    public class WebSocketMessage
    {
        public WebSocketEvent Event { get; set; }
        public string Message { get; set; }
    }
    public enum ResourceType
    {
        Coins,
        Rolls
    }

    public class AuthentificatedPlayer
    {
        public string PlayerId { get; set; }
        public string AuthToken { get; set; }

        public AuthentificatedPlayer(string playerId, string authToken) {
            PlayerId = playerId;
            AuthToken = authToken;
        }
    }
    public class GameContext
    {
        private readonly ILogger<GameContext> logger;
        private readonly AppServiceAPI appService;
        private readonly UrlProvider urlProvider;

        private AuthentificatedPlayer currentPlayer;
        private ClientWebSocket webSocketConnection;

        private bool connectingToWebSocketInProgress;

        public bool IsPlayerAuthentificated { get { return currentPlayer != null; } }

        public GameContext(ILogger<GameContext> logger, AppServiceAPI appService, UrlProvider urlProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.appService = appService ?? throw new ArgumentNullException(nameof(appService));
            this.urlProvider = urlProvider ?? throw new ArgumentNullException(nameof(urlProvider));
        }

        #region Player Actions
        public async Task AuthentificatePlayer(string deviceId)
        {
            var loginRequest = new LoginQuery.QueryRequest { DeviceId = deviceId };

            await appService.PostRequest<LoginQuery.QueryResponse>("/api/userauthentification/login", loginRequest, async (response) =>
            {
                if (response != null && response.Status == HttpStatusCode.OK)
                {
                    appService.SetCurrentAuthToken(response.AuthToken);

                    if (response.AlreadyOnline)
                    {
                        logger.LogWarning("You are already logged in!");
                        logger.LogInformation($"PlayerId: {response.PlayerId}");
                        return;
                    }

                    logger.LogInformation("Login successfull!");
                    logger.LogInformation($"PlayerId: {response.PlayerId}");
                    
                    currentPlayer = new AuthentificatedPlayer(response.PlayerId, response.AuthToken);

                    ConnectAndListenToWebsocket();
                }
            });

            while (connectingToWebSocketInProgress)
                await Task.Delay(1);
        }
        public async Task SendGift(string toPlayerId, ResourceType resourceType, double resourceValue)
        {
            if(currentPlayer == null)
            {
                logger.LogError($"You are not logged in!");
                return;
            }

            var sendGiftRequest = new SendGiftQuery.QueryRequest
            {
                FriendPlayerId = toPlayerId,
                ResourceType = resourceType,
                ResourceValue = resourceValue,
            };

            await appService.PostRequest<SendGiftQuery.QueryResponse>("/api/gifts/sendgift", sendGiftRequest, (response) =>
            {
                if (response != null && response.Status == HttpStatusCode.OK)
                    logger.LogInformation("Gift sent successfully!");
            });
        }
        public async Task GetResource(ResourceType resourceType)
        {
            if (currentPlayer == null)
            {
                logger.LogError($"You are not logged in!");
                return;
            }

            var getResourceRequest = new GetResourceQuery.QueryRequest
            {
                ResourceType = resourceType,
            };

            await appService.GetRequest<GetResourceQuery.QueryResponse>("/api/resources/getresource", getResourceRequest, (response) =>
            {
                if (response != null && response.Status == HttpStatusCode.OK)
                    logger.LogInformation($"Resource value: {response.ResourceValue}");
            });
        }
        public async Task UpdateResources(ResourceType resourceType, double newValue)
        {
            if (currentPlayer == null)
            {
                logger.LogError($"You are not logged in!");
                return;
            }

            var updateResourceRequest = new UpdateResourcesQuery.QueryRequest
            {
                ResourceType = resourceType,
                ResourceValue = newValue,
            };

            await appService.PostRequest<UpdateResourcesQuery.QueryResponse>("/api/resources/updateresources", updateResourceRequest, (response) =>
            {
                if (response != null && response.Status == HttpStatusCode.OK)
                {
                    logger.LogInformation("New balance:");
                    foreach(var res in response.UpdatedResources)
                        logger.LogInformation($"{res.ResourceType}: {res.Value}");
                }
                    
            });
        }
        #endregion

        private async void ConnectAndListenToWebsocket()
        {
            logger.LogInformation($"Connecting to web socket...");
            connectingToWebSocketInProgress = true;

            using (var clientWebSocket = new ClientWebSocket())
            {
                clientWebSocket.Options.SetRequestHeader("Authorization", currentPlayer.AuthToken);
                // Connect to the WebSocket server
                await clientWebSocket.ConnectAsync(new Uri(urlProvider.WebSocketBaseUrl), CancellationToken.None);

                if (clientWebSocket.State != WebSocketState.Open)
                {
                    logger.LogError($"Connection to web socket failed!");
                    connectingToWebSocketInProgress = false;
                    return;
                }
                else
                    logger.LogInformation($"Connection to web socket completed!");

                connectingToWebSocketInProgress = false;

                webSocketConnection = clientWebSocket;

                // Receive messages from the server
                byte[] buffer = new byte[1024];

                var receiveTask = Task.Run(async () =>
                {
                    while (clientWebSocket.State == WebSocketState.Open)
                    {
                        var receiveResult = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (receiveResult.MessageType == WebSocketMessageType.Text)
                        {
                            var msg = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                            var msgObject = JsonSerializer.Deserialize<WebSocketMessage>(msg);

                            MessageReceived(msgObject);
                        }
                    }

                    logger.LogInformation($"Connection to web socket closed!");
                });
                await receiveTask;

                // Close the WebSocket connection
                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
        }
        private void MessageReceived(WebSocketMessage msg)
        {
            if (msg == null)
            {
                logger.LogError("Received a bad formatted web socket msg!");
                return;
            }

            logger.LogInformation($"Received msg {msg.Event} {msg.Message}");
        }
    }
}
