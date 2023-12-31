﻿using Microsoft.Extensions.Logging;
using SoareAlexConsoleApp.Services;
using SoareAlexConsoleApp.Services.Game;

namespace SoareAlexConsoleApp.Commands.Handlers
{
    public class SetCustomUrlCommandHandler : ICommandHandler
    {
        public static string CommandName { get { return "/setcustomurl"; } }
        public static string CommandInfo { 
            get 
            { 
                return "<ServerUrl> <WebsocketUrl>" +
                    "\n   Can only be called before logging in!"; 
            } 
        }

        private readonly ILogger<LoginCommandHandler> logger;
        private readonly UrlProvider urlProvider;
        private readonly GameContext gameContext;

        public SetCustomUrlCommandHandler(ILogger<LoginCommandHandler> logger, UrlProvider urlProvider, GameContext gameContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.urlProvider = urlProvider ?? throw new ArgumentNullException(nameof(urlProvider));
            this.gameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
        }

        public async Task Handle(List<string> parameters)
        {
            if (parameters == null || parameters.Count < 2)
            {
                logger.LogError($"Not enough parameters supplied!");
                logger.LogError($"Expected parameters: {CommandInfo}");
                return;
            }

            if (gameContext.IsPlayerAuthentificated)
            {
                logger.LogError($"This command can only be used before logging in!");
                return;
            }

            logger.LogInformation($"Custom url successfully set!");

            var baseUrl = parameters[0];
            var webSocketUrl = parameters[1];

            urlProvider.SetCustomUrl(baseUrl, webSocketUrl);
        }
    }
}
