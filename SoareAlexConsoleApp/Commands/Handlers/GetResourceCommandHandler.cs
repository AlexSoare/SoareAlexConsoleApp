using Microsoft.Extensions.Logging;

namespace SoareAlexConsoleApp.Commands.Handlers
{
    public class GetResourceCommandHandler : AbstractCommandHandler
    {
        public new static string CommandName { get { return "/getresource"; } }
        public new static string CommandInfo { get { return "<ResourceType(Coins,Rolls)>"; } }

        private readonly ILogger<GetResourceCommandHandler> logger;
        private readonly GameContext gameContext;

        public GetResourceCommandHandler(ILogger<GetResourceCommandHandler> logger, GameContext gameContext)
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

            ResourceType resourceType;
            if (!Enum.TryParse(parameters[0], out resourceType))
            {
                logger.LogError($"Cannot parse {parameters[0]} as a ResourceType!");
                return;
            }

            await gameContext.GetResource(resourceType);
        }
    }
}
