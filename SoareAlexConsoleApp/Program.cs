using Microsoft.Extensions.DependencyInjection;
using SoareAlexConsoleApp;
using SoareAlexConsoleApp.Commands;

var serviceProvider = DIContainerManager.Configure();

while (true)
{
    Console.Write("Enter a command: ");
    string input = Console.ReadLine();

    var handleService = serviceProvider.GetService<CommandsHandlerService>();

    if (handleService == null)
        return;

    var command = handleService.ParseCommand(input);
    if (command != null)
    {
        await handleService.HandleCommandAsync(command);
    }
}