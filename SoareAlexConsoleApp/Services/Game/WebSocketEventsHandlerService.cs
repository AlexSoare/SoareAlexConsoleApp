using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using SoareAlexConsoleApp.Services.Game.WebSocket;

namespace SoareAlexConsoleApp.Services.Game
{
    public class WebSocketEventsHandlerService
    {
        private Dictionary<WebSocketEventType, List<IWebSocketMsgListener>> webSocketEventListeners = new Dictionary<WebSocketEventType, List<IWebSocketMsgListener>>();
        
        public void AddWebSocketEventListener<T>(WebSocketEventType type, Action<T> callback)
        {

            if (!webSocketEventListeners.ContainsKey(type))
                webSocketEventListeners.Add(type, new List<IWebSocketMsgListener>());

            var listener = new WebSocketEventListener<T>(callback);

            webSocketEventListeners[type].Add(listener);
        }

        public void ProcessWebSocketEvent(string eventJson)
        {
            var msgObj = JsonConvert.DeserializeObject<RawWebSocketEvent>(eventJson);

            var msgType = (WebSocketEventType)Enum.Parse(typeof(WebSocketEventType), msgObj.Type);

            if (webSocketEventListeners.ContainsKey(msgType))
                foreach (var listener in webSocketEventListeners[msgType])
                    listener.Trigger(msgObj.Event);
        }
    }
}
