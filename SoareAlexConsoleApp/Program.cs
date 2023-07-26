using Microsoft.Extensions.DependencyInjection;
using SoareAlexConsoleApp;
using SoareAlexConsoleApp.Services;

var serviceProvider = DIContainerManager.Configure();

while (true)
{
    Console.Write("Enter a command: ");
    string input = Console.ReadLine();

    var handleCommandsService = serviceProvider.GetService<CommandsHandlerService>();

    if (handleCommandsService == null)
        return;

    var command = handleCommandsService.ParseCommand(input);
    if (command != null)
        await handleCommandsService.HandleCommandAsync(command);
}