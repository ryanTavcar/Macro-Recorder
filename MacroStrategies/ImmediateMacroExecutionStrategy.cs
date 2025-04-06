using Names.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;

namespace Names.MacroStrategies
{
    public class ImmediateMacroExecutionStrategy : BaseMacroExecutionStrategy
    {
        public ImmediateMacroExecutionStrategy(IInputSimulator simulator, Action<string> logger)
            : base(simulator, logger)
        {
        }

        public override void Execute(IList<MacroCommandViewModel> commands, int loopCount, CancellationToken token)
        {
            logger("Starting macro execution...");
            ExecuteMacroSequence(commands, loopCount, token);
        }
    }
}
