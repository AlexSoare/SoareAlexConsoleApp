namespace SoareAlexConsoleApp.Services.Game.WebSocket
{
    public abstract class BaseEventListener<T>
    {
        public abstract void OnEvent(T eventData);
    }
}
