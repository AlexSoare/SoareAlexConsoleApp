using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoareAlexConsoleApp.Commands.Handlers
{
    public abstract class AbstractCommandHandler
    {
        public static string CommandName { get; }

        public abstract Task Handle(List<string> parameters);
    }
}
