using Names.Helpers;
using Names.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;
using WindowsInput;
using System.Windows;

namespace Names.MacroStrategies
{
    public class TriggerKeyMacroExecutionStrategy : BaseMacroExecutionStrategy
    {
        private readonly string triggerKey;
        private CancellationTokenSource internalCts;

        public TriggerKeyMacroExecutionStrategy(IInputSimulator simulator, Action<string> logger, string triggerKey)
            : base(simulator, logger)
        {
            this.triggerKey = triggerKey;
        }

        public override void Execute(IList<MacroCommandViewModel> commands, int loopCount, CancellationToken token)
        {
            logger($"Waiting for trigger key ({triggerKey})...");

            internalCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            var combinedToken = internalCts.Token;

            // Start a task to monitor for trigger key presses
            Task.Run(() =>
            {
                bool wasKeyDown = false;

                // Keep monitoring until cancelled
                while (!combinedToken.IsCancellationRequested)
                {
                    try
                    {
                        VirtualKeyCode triggerKeyCode = KeyboardUtility.ConvertToVirtualKeyCode(triggerKey);

                        // Check if key state changed from up to down (key just pressed)
                        bool isKeyDown = simulator.InputDeviceState.IsKeyDown(triggerKeyCode);

                        if (isKeyDown && !wasKeyDown)
                        {
                            // Key was just pressed, execute macro once
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                logger($"Trigger key ({triggerKey}) pressed! Executing macro...");
                                ExecuteMacroSequence(commands, loopCount, combinedToken);
                            });
                        }

                        // Update previous state
                        wasKeyDown = isKeyDown;
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            logger($"Error monitoring trigger key: {ex.Message}");
                        });
                        break;
                    }

                    // Small delay to reduce CPU usage
                    Thread.Sleep(50);
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    logger("Macro monitoring stopped.");
                });

            }, combinedToken);
        }

        public override void Stop()
        {
            internalCts?.Cancel();
        }
    }
}
