using Microsoft.Extensions.Logging;

namespace SoareAlexConsoleApp.Commands.Handlers
{
    public class UpdateResourcesCommandHandler : AbstractCommandHandler
    {
        public new static string CommandName { get { return "/updateresources"; } }
        public new static string CommandInfo { get { return "<ResourceType(Coins,Rolls)> <ResourceValue>"; } }

        private readonly ILogger<UpdateResourcesCommandHandler> logger;
        private readonly GameContext gameContext;

        public UpdateResourcesCommandHandler(ILogger<UpdateResourcesCommandHandler> logger, GameContext gameContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.gameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
        }

        public override async Task Handle(List<string> parameters)
        {
            if (parameters == null || parameters.Count < 2)
            {
                logger.LogError($"Not enough parameters supplied!");
                logger.LogError($"Expected parameters: {CommandInfo}");
                return;
            }

            ResourceType resourceType;
            if(!Enum.TryParse(parameters[0], out resourceType))
                logger.LogError($"Cannot parse {parameters[0]} as a ResourceType!");

            double resourceValue;
            if (!double.TryParse(parameters[1], out resourceValue))
                logger.LogError($"Cannot parse {parameters[1]} as a double ResourceValue!");

            await gameContext.UpdateResources(resourceType, resourceValue);
        }
    }
}
