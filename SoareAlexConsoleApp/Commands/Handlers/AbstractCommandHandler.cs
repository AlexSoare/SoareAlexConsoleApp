namespace SoareAlexConsoleApp.Commands.Handlers
{
    public abstract class AbstractCommandHandler
    {
        public static string CommandName { get; }
        public static string CommandInfo { get; }

        public abstract Task Handle(List<string> parameters);
    }
}
