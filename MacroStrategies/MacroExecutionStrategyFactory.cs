using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;

namespace Names.MacroStrategies
{
    public static class MacroExecutionStrategyFactory
    {
        public static IMacroExecutionStrategy Create(bool waitForTrigger, string triggerKey, IInputSimulator simulator, Action<string> logger)
        {
            if (waitForTrigger)
            {
                return new TriggerKeyMacroExecutionStrategy(simulator, logger, triggerKey);
            }
            else
            {
                return new ImmediateMacroExecutionStrategy(simulator, logger);
            }
        }
    }
}
