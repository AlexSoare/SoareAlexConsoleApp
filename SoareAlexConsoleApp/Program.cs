using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;

Console.WriteLine("Hello, World!");

string serverUrl = "wss://localhost:7131?alex=alex&veri=x32s23&playerId=123"; // Replace with your server URL

using (var clientWebSocket = new ClientWebSocket())
{
    // Connect to the WebSocket server
    await clientWebSocket.ConnectAsync(new Uri(serverUrl), CancellationToken.None);

    // Send a message to the server (you can customize the message as needed)
    string message = "1";
    await SendTextMessage(message);

    // Receive messages from the server
    byte[] buffer = new byte[1024];
    while (clientWebSocket.State == WebSocketState.Open)
    {
        //var receiveResult = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //if (receiveResult.MessageType == WebSocketMessageType.Text)
        //{
        //    var msg = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
        //    var nr = int.Parse(message);
        //    nr = nr + 1;
        //    await SendTextMessage(nr.ToString());
        //}
    }

    // Close the WebSocket connection
    await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);

    async Task SendTextMessage(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        await clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}