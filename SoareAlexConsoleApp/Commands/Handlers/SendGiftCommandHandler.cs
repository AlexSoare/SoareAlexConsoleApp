using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SoareAlexConsoleApp.Commands.Handlers
{
    public class SendGiftCommandHandler : AbstractCommandHandler
    {
        public new static string CommandName { get { return "/sendgift"; } }

        private readonly AppServiceAPI appService;
        private readonly GameContext gameContext;

        public SendGiftCommandHandler(AppServiceAPI appService, GameContext gameContext)
        {
            this.appService = appService ?? throw new ArgumentNullException(nameof(appService));
            this.gameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
        }

        public override async Task Handle(List<string> parameters)
        {
            Console.WriteLine("Send gift called");

            if (parameters == null || parameters.Count < 1)
                return;
            //string serverUrl = $"wss://localhost:7131";//?authToken={parameters[0]}"; // Replace with your server URL

            //using (var clientWebSocket = new ClientWebSocket())
            //{
            //    clientWebSocket.Options.SetRequestHeader("Authorization", parameters[0]);
            //    // Connect to the WebSocket server
            //    await clientWebSocket.ConnectAsync(new Uri(serverUrl), CancellationToken.None);

            //    // Receive messages from the server
            //    byte[] buffer = new byte[1024];
            //    var receiveTask = Task.Run(async () =>
            //    {
            //        while (clientWebSocket.State == WebSocketState.Open)
            //        {
            //            var receiveResult = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            //            if (receiveResult.MessageType == WebSocketMessageType.Text)
            //            {
            //                var msg = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            //                var msgObject = JsonSerializer.Deserialize<WebSocketMessage>(msg);

            //                Console.WriteLine("Received msg: " + msgObject.Event + "|" + msgObject.Message);
            //            }
            //        }
            //    });
            //    //await receiveTask;

            //    // Close the WebSocket connection
            //    await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);

            //    async Task SendTextMessage(string message)
            //    {
            //        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            //        await clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            //    }
            //}
        }
    }
}
