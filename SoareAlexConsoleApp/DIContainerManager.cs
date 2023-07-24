using Microsoft.Extensions.DependencyInjection;
using SoareAlexConsoleApp.Commands;
using SoareAlexConsoleApp.Commands.Handlers;
using System.Reflection;

namespace SoareAlexConsoleApp
{
    public static class DIContainerManager
    {
        public static ServiceProvider Configure()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped(s =>
            {
                return new AppServiceAPI("https://localhost:7131");
            });

            serviceCollection.AddScoped<CommandsHandlerService>();

            CommandsHandlerService.LogAvailableCommands();

            var commandHandlerTypes = Assembly.GetExecutingAssembly()
               .GetTypes()
               .Where(type => typeof(AbstractCommandHandler).IsAssignableFrom(type) && !type.IsAbstract);
            foreach (var commandHandlerType in commandHandlerTypes)
            {
                serviceCollection.AddScoped(commandHandlerType);
            }

            serviceCollection.AddSingleton<GameContext>();
           
            var provider = serviceCollection.BuildServiceProvider();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
