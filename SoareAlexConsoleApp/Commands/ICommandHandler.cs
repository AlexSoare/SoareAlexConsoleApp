namespace SoareAlexConsoleApp.Commands
{
    public interface ICommandHandler
    {
        Task Handle(List<string> parameters);
    }
}
