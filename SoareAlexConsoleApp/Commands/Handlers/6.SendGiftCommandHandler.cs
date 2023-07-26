using Microsoft.Extensions.Logging;
using SoareAlexConsoleApp.Services.Game;

namespace SoareAlexConsoleApp.Commands.Handlers
{
    public class SendGiftCommandHandler : ICommandHandler
    {
        public static string CommandName { get { return "/sendgift"; } }
        public static string CommandInfo { get { return "<PlayerId> <ResourceType(Coins,Rolls)> <ResourceValue>"; } }

        private readonly ILogger<SendGiftCommandHandler> logger;
        private readonly GameContext gameContext;

        public SendGiftCommandHandler(ILogger<SendGiftCommandHandler> logger, GameContext gameContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.gameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
        }

        public async Task Handle(List<string> parameters)
        {
            if (parameters == null || parameters.Count < 3)
            {
                logger.LogError($"Not enough parameters supplied!");
                logger.LogError($"Expected parameters: {CommandInfo}");
                return;
            }

            var friendPlayerId = parameters[0];

            ResourceType resourceType;
            if (!Enum.TryParse(parameters[1], out resourceType))
            {
                logger.LogError($"Cannot parse {parameters[1]} as a ResourceType!");
                return;
            }

            double resourceValue;
            if (!double.TryParse(parameters[2], out resourceValue))
            {
                logger.LogError($"Cannot parse {parameters[2]} as a double ResourceValue!");
                return;
            }

            await gameContext.SendGift(friendPlayerId, resourceType, resourceValue);
        }
    }
}
