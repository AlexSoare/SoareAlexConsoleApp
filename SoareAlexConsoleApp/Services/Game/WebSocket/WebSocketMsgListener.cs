using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoareAlexConsoleApp.Services.Game.WebSocket
{
    public enum WebSocketEventType
    {
        Gift
    }

    public interface IWebSocketMsgListener
    {
        int GetHash();
        void Trigger(string json);
    }

    public class WebSocketEventListener<T> : IWebSocketMsgListener
    {
        public Action<T> Callback;

        public WebSocketEventListener(Action<T> callback)
        {
            Callback = callback;
        }

        public void Trigger(string json)
        {
            var msgObj = JsonConvert.DeserializeObject<T>(json);
            Callback(msgObj);
        }

        public int GetHash()
        {
            return Callback.GetHashCode();
        }
    }
}
