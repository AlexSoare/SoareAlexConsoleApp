using System.Reflection;
using Microsoft.Extensions.Logging;
using SoareAlexConsoleApp.Commands.Handlers;

namespace SoareAlexConsoleApp.Commands
{
    public class CommandsHandlerService
    {
        private readonly ILogger<CommandsHandlerService> logger;

        private Dictionary<string, AbstractCommandHandler> commandHandlers;
        private List<string> availableCommands;

        public CommandsHandlerService(ILogger<CommandsHandlerService> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            commandHandlers = new Dictionary<string, AbstractCommandHandler>();
            availableCommands = new List<string>();

            var commandHandlerTypes = Assembly.GetExecutingAssembly()
              .GetTypes()
              .Where(type => typeof(AbstractCommandHandler).IsAssignableFrom(type) && !type.IsAbstract);

            if (commandHandlerTypes != null)
            {
                foreach (var commandHandlerType in commandHandlerTypes)
                {
                    var commandProperty = commandHandlerType.GetProperty("CommandName", BindingFlags.Public | BindingFlags.Static);
                    if (commandProperty != null)
                    {
                        var commandValue = (string)commandProperty.GetValue(null);
                        if (!string.IsNullOrEmpty(commandValue))
                        {
                            if(!availableCommands.Contains(commandValue))
                            {
                                commandHandlers.Add(commandValue, serviceProvider.GetService(commandHandlerType) as AbstractCommandHandler);
                                availableCommands.Add(commandValue);
                            }
                        }
                    }
                }
            }
        }

        public Command ParseCommand(string input)
        {
            // Split the input into command name and parameters
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return null;
            }

            // The first part is the command name, and the rest are parameters
            var command = new Command
            {
                Name = parts[0],
                Parameters = parts.Skip(1).ToList()
            };

            return command;
        }
        public async Task HandleCommandAsync(Command command)
        {
            var commandHandler = GetCommandHandler(command.Name.ToLower());
            if (commandHandler != null)
            {
                await commandHandler.Handle(command.Parameters);
            }
            else
            {
                logger.LogError("Invalid command. Available commands:");
                foreach (var c in availableCommands)
                    logger.LogInformation(c);
            }
        }
        private AbstractCommandHandler GetCommandHandler(string forCommmand)
        {
            AbstractCommandHandler handler;

            if (commandHandlers.TryGetValue(forCommmand, out handler))
                return handler;

            return null;
        }

        public void LogAvailableCommands()
        {
            var tempCommandsList = new List<string>();

            var commandHandlerTypes = Assembly.GetExecutingAssembly()
             .GetTypes()
             .Where(type => typeof(AbstractCommandHandler).IsAssignableFrom(type) && !type.IsAbstract);

            if (commandHandlerTypes != null)
            {
                foreach (var commandHandlerType in commandHandlerTypes)
                {
                    var commandProperty = commandHandlerType.GetProperty("CommandName", BindingFlags.Public | BindingFlags.Static);
                    var commandInfoProperty = commandHandlerType.GetProperty("CommandInfo", BindingFlags.Public | BindingFlags.Static);

                    if (commandProperty != null)
                    {
                        var commandValue = (string)commandProperty.GetValue(null);

                        if (string.IsNullOrEmpty(commandValue))
                            LogBrokenCommand(commandHandlerType, "\"CommandName\" paramenter is empty or not defined");
                        else if (tempCommandsList.Contains(commandValue))
                            LogBrokenCommand(commandHandlerType, $"Handler for {commandValue} was already registered!");
                        else
                        {
                            var commandInfo = "";

                            if(commandInfoProperty != null)
                                commandInfo = (string)commandInfoProperty.GetValue(null);

                            tempCommandsList.Add(commandValue + " " + commandInfo);
                        }
                    }
                    else
                        LogBrokenCommand(commandHandlerType, "\"CommandName\" paramenter is empty or not defined");
                }
            }

            Console.WriteLine("Available commands:");
            Console.WriteLine("");
            foreach (var c in tempCommandsList)
                Console.WriteLine(c);

            Console.WriteLine("");
        }
        private void LogBrokenCommand(Type commandType, string reasonMessage)
        {
            var classNameSplit = commandType.Name.Split('.');
            var className = classNameSplit != null && classNameSplit.Length > 0 ? classNameSplit.Last() : "NAME_NOT_FOUND";

            logger.LogError($"Could not register {className} command because:");
            logger.LogError(reasonMessage);
        }
    }
}
