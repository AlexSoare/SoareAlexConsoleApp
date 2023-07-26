using Microsoft.Extensions.Logging;
using static SoareAlexConsoleApp.Services.Game.WebSocket.GiftEventListener;

namespace SoareAlexConsoleApp.Services.Game.WebSocket
{
    public class GiftEventListener : BaseEventListener<GiftEventData>
    {
        public class GiftEventData
        {
            public string SenderId { get; set; }
            public ResourceType ResourceType { get; set; }
            public double ResourceValue { get; set; }
        }

        private readonly ILogger<GiftEventListener> logger;

        public GiftEventListener(WebSocketEventsHandlerService eventsProcessor, ILogger<GiftEventListener> logger)
        {
            eventsProcessor.AddWebSocketEventListener<GiftEventData>(WebSocketEventType.Gift, OnEvent);
            this.logger = logger;
        }

        public override void OnEvent(GiftEventData eventData)
        {
            logger.LogInformation($"Gift event received: {eventData.ResourceValue} {eventData.ResourceType} from {eventData.SenderId}");
        }
    }
}
