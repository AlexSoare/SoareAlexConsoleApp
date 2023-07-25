using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoareAlexConsoleApp.Commands;
using SoareAlexConsoleApp.Commands.Handlers;
using System.Reflection;
using Serilog;

namespace SoareAlexConsoleApp
{
    public static class DIContainerManager
    {
        public static ServiceProvider Configure()
        {
            var serviceCollection = new ServiceCollection();

            Log.Logger = new LoggerConfiguration()
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .CreateLogger();

            serviceCollection.AddLogging(builder =>
            {
                builder.AddSerilog(dispose: true);
            });

            serviceCollection.AddSingleton<UrlProvider>();
            serviceCollection.AddSingleton<GameContext>(p =>
            {
                var appService = p.GetService<AppServiceAPI>();
                var logger = p.GetService<ILogger<GameContext>>();
                var urlProvider = p.GetService<UrlProvider>();

                return new GameContext(logger, appService, urlProvider);
            });
            serviceCollection.AddSingleton(p =>
            {
                var logger = p.GetService<ILogger<AppServiceAPI>>();
                var urlProvider = p.GetService<UrlProvider>();

                return new AppServiceAPI(logger, urlProvider);
            });

            serviceCollection.AddScoped<CommandsHandlerService>();
           
            var commandHandlerTypes = Assembly.GetExecutingAssembly()
               .GetTypes()
               .Where(type => typeof(AbstractCommandHandler).IsAssignableFrom(type) && !type.IsAbstract);
            foreach (var commandHandlerType in commandHandlerTypes)
            {
                serviceCollection.AddScoped(commandHandlerType);
            }

            var provider = serviceCollection.BuildServiceProvider();

            provider.GetService<CommandsHandlerService>().LogAvailableCommands();

            return provider;
        }
    }
}
