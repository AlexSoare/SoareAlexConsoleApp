using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Serilog;
using SoareAlexConsoleApp.Services;
using SoareAlexConsoleApp.Commands;
using SoareAlexConsoleApp.Services.AppService;
using SoareAlexConsoleApp.Services.Game;
using SoareAlexConsoleApp.Services.Game.WebSocket;

namespace SoareAlexConsoleApp
{
    public static class DIContainerManager
    {
        public static ServiceProvider Configure()
        {
            var serviceCollection = new ServiceCollection();

            // Register logger
            Log.Logger = new LoggerConfiguration()
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .CreateLogger();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddSerilog(dispose: true);
            });

            // Register services
            serviceCollection.AddSingleton<UrlProvider>();
            serviceCollection.AddSingleton<GameContext>(p =>
            {
                var appService = p.GetService<AppServiceAPI>();
                var logger = p.GetService<ILogger<GameContext>>();
                var urlProvider = p.GetService<UrlProvider>();
                var webSocketEventsHandlerService = p.GetService<WebSocketEventsHandlerService>();

                return new GameContext(logger, appService, urlProvider, webSocketEventsHandlerService);
            });
            serviceCollection.AddSingleton(p =>
            {
                var logger = p.GetService<ILogger<AppServiceAPI>>();
                var urlProvider = p.GetService<UrlProvider>();

                return new AppServiceAPI(logger, urlProvider);
            });
            serviceCollection.AddSingleton<WebSocketEventsHandlerService>();

            serviceCollection.AddScoped<CommandsHandlerService>();
           
            // Register commands
            var commandHandlerTypes = Assembly.GetExecutingAssembly()
               .GetTypes()
               .Where(type => typeof(ICommandHandler).IsAssignableFrom(type) && !type.IsAbstract);
            foreach (var commandHandlerType in commandHandlerTypes)
            {
                serviceCollection.AddScoped(commandHandlerType);
            }

            var provider = serviceCollection.BuildServiceProvider();

            // Register web socket listeners here
            var webSocketEventsHandlerService = provider.GetService<WebSocketEventsHandlerService>();

            var giftLogger = provider.GetService<ILogger<GiftEventListener>>();
            new GiftEventListener(webSocketEventsHandlerService, giftLogger);

            // Log available commands
            provider.GetService<CommandsHandlerService>().LogAvailableCommands();

           
            return provider;
        }
    }
}
