using Names.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Names.MacroStrategies
{
    public interface IMacroExecutionStrategy
    {
        void Execute(IList<MacroCommandViewModel> commands, int loopCount, CancellationToken token);
        void Stop();
    }
}
