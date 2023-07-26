using Microsoft.Extensions.Logging;
using SoareAlexConsoleApp.Services;
using SoareAlexConsoleApp.Services.Game;

namespace SoareAlexConsoleApp.Commands.Handlers
{
    public class SetUrlEnviromentCommandHandler : ICommandHandler
    {
        public static string CommandName { get { return "/seturlenvironment"; } }
        public static string CommandInfo
        {
            get
            {
                return "<EnvironmentType(Development,Production)>" +
                    "\n   Development = https://localhost:7131" +
                    "\n   Production = https://soarenitagameserver.azurewebsites.net" +
                    "\n   Can only be called before logging in!" +
                    "\n   Default is Development!";
            }
        }

        private readonly ILogger<LoginCommandHandler> logger;
        private readonly UrlProvider urlProvider;
        private readonly GameContext gameContext;

        public SetUrlEnviromentCommandHandler(ILogger<LoginCommandHandler> logger, UrlProvider urlProvider, GameContext gameContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.urlProvider = urlProvider ?? throw new ArgumentNullException(nameof(urlProvider));
            this.gameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
        }

        public async Task Handle(List<string> parameters)
        {
            if (parameters == null || parameters.Count < 1)
            {
                logger.LogError($"Not enough parameters supplied!");
                logger.LogError($"Expected parameters: {CommandInfo}");
                return;
            }

            if (gameContext.IsPlayerAuthentificated)
            {
                logger.LogError($"This command can be used ONLY before loggin in!");
                return;
            }

            EnvironmentType enviromentType;
            if (!Enum.TryParse(parameters[0], out enviromentType))
            {
                logger.LogError($"Cannot parse {parameters[0]} as a EnvironmentType!");
                return;
            }

            logger.LogInformation($"{enviromentType} environment successfuly set!");

            urlProvider.SetUrlType(enviromentType);
        }
    }
}
