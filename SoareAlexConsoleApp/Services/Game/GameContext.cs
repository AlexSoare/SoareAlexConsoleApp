using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SoareAlexConsoleApp.Services.AppService;
using SoareAlexConsoleApp.Services.AppServiceAPIs.Queries;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace SoareAlexConsoleApp.Services.Game
{
    public enum ResourceType
    {
        Coins,
        Rolls
    }

    public class AuthentificatedPlayer
    {
        public string PlayerId { get; set; }
        public string AuthToken { get; set; }

        public AuthentificatedPlayer(string playerId, string authToken)
        {
            PlayerId = playerId;
            AuthToken = authToken;
        }
    }

    public class GameContext
    {
        private readonly ILogger<GameContext> logger;
        private readonly AppServiceAPI appService;
        private readonly UrlProvider urlProvider;
        private readonly WebSocketEventsHandlerService webSocketEventProcessor;

        private AuthentificatedPlayer currentPlayer;
        private ClientWebSocket webSocketConnection;

        private bool connectingToWebSocketInProgress;

        public bool IsPlayerAuthentificated { get { return currentPlayer != null; } }

        public GameContext(ILogger<GameContext> logger, AppServiceAPI appService, UrlProvider urlProvider, WebSocketEventsHandlerService webSocketEventProcessor)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.appService = appService ?? throw new ArgumentNullException(nameof(appService));
            this.urlProvider = urlProvider ?? throw new ArgumentNullException(nameof(urlProvider));
            this.webSocketEventProcessor = webSocketEventProcessor ?? throw new ArgumentNullException(nameof(webSocketEventProcessor));
        }

        #region Player Actions
        public async Task AuthentificatePlayer(string deviceId)
        {
            var loginRequest = new Login.QueryRequest { DeviceId = deviceId };

            await appService.PostRequest<Login.QueryResponse>("/api/userauthentification/login", loginRequest, async (response) =>
            {
                if (response != null && response.Status == HttpStatusCode.OK)
                {
                    appService.SetAuthToken(response.AuthToken);

                    if (response.AlreadyOnline)
                    {
                        logger.LogWarning("You are already logged in!");
                        logger.LogInformation($"PlayerId: {response.PlayerId}");
                        return;
                    }

                    logger.LogInformation("Login successful!");
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
            if (currentPlayer == null)
            {
                logger.LogError($"You are not logged in!");
                return;
            }

            var sendGiftRequest = new SendGift.QueryRequest
            {
                FriendPlayerId = toPlayerId,
                ResourceType = resourceType,
                ResourceValue = resourceValue,
            };

            await appService.PostRequest<SendGift.QueryResponse>("/api/gifts/sendgift", sendGiftRequest, (response) =>
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

            var getResourceRequest = new GetResource.QueryRequest
            {
                ResourceType = resourceType,
            };

            await appService.GetRequest<GetResource.QueryResponse>("/api/resources/getresource", getResourceRequest, (response) =>
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

            var updateResourceRequest = new UpdateResources.QueryRequest
            {
                ResourceType = resourceType,
                ResourceValue = newValue,
            };

            await appService.PostRequest<UpdateResources.QueryResponse>("/api/resources/updateresources", updateResourceRequest, (response) =>
            {
                if (response != null && response.Status == HttpStatusCode.OK)
                {
                    logger.LogInformation("New balance:");
                    foreach (var res in response.UpdatedResources)
                        logger.LogInformation($"{res.ResourceType}: {res.Value}");
                }

            });
        }
        #endregion

        private void Disconnect()
        {
            webSocketConnection = null;
            currentPlayer = null;
            logger.LogInformation($"Connection to web socket lost, logging off!");
        }
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

                try
                {
                    var receiveTask = Task.Run(async () =>
                        {
                            while (clientWebSocket.State == WebSocketState.Open)
                            {
                                byte[] buffer = new byte[1024];
                                var receiveResult = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                                if (receiveResult.MessageType == WebSocketMessageType.Text)
                                {

                                    var msg = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);

                                    try
                                    {
                                        webSocketEventProcessor.ProcessWebSocketEvent(msg);
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.LogWarning($"Web socket received a bad formatted msg: {msg}");
                                    }

                                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                                    {
                                        await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                                        Disconnect();
                                    }
                                }
                            }
                        });
                    await receiveTask;
                    logger.LogInformation($"Connection to web socket closed!");
                }
                catch (WebSocketException)
                {
                    Disconnect();
                }
            }
        }
    }
}
