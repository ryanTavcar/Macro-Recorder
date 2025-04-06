using Names.Helpers;
using Names.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;
using WindowsInput;

namespace Names.MacroStrategies
{
    public abstract class BaseMacroExecutionStrategy : IMacroExecutionStrategy
    {
        protected readonly IInputSimulator simulator;
        protected readonly Action<string> logger;

        public BaseMacroExecutionStrategy(IInputSimulator simulator, Action<string> logger)
        {
            this.simulator = simulator;
            this.logger = logger;
        }

        public abstract void Execute(IList<MacroCommandViewModel> commands, int loopCount, CancellationToken token);

        public virtual void Stop()
        {
            // Default implementation is empty
        }

        protected void ExecuteMacroSequence(IList<MacroCommandViewModel> commands, int loopCount, CancellationToken token)
        {
            for (int loop = 0; loop < loopCount; loop++)
            {
                if (token.IsCancellationRequested)
                    break;

                logger($"Loop {loop + 1} of {loopCount}");

                foreach (var command in commands)
                {
                    if (token.IsCancellationRequested)
                        break;

                    // Wait for the specified delay
                    Thread.Sleep(command.DelayMs);

                    try
                    {
                        VirtualKeyCode keyCode = KeyboardUtility.ConvertToVirtualKeyCode(command.KeyName);
                        simulator.Keyboard.KeyPress(keyCode);
                        logger($"Executed: {command.KeyName}");
                    }
                    catch (Exception ex)
                    {
                        logger($"Error executing key {command.KeyName}: {ex.Message}");
                    }
                }

                if (loop < loopCount - 1 && !token.IsCancellationRequested)
                {
                    logger("Pausing before next loop...");
                    Thread.Sleep(500);
                }
            }

            if (!token.IsCancellationRequested)
            {
                logger("Macro execution completed");
            }
        }
    }
}
