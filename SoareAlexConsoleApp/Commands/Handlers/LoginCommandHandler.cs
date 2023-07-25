using Microsoft.Extensions.Logging;

namespace SoareAlexConsoleApp.Commands.Handlers
{
    public class LoginCommandHandler : AbstractCommandHandler
    {
        public new static string CommandName { get { return "/login"; } }
        public new static string CommandInfo { get { return "<DeviceId>"; } }

        private readonly ILogger<LoginCommandHandler> logger;
        private readonly GameContext gameContext;

        public LoginCommandHandler(ILogger<LoginCommandHandler> logger, GameContext gameContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.gameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
        }

        public override async Task Handle(List<string> parameters)
        {
            if (parameters == null || parameters.Count < 1)
            {
                logger.LogError($"Not enough parameters supplied!");
                logger.LogError($"Expected parameters: {CommandInfo}");
                return;
            }

            await gameContext.AuthentificatePlayer(parameters[0]);
        }
    }
}
