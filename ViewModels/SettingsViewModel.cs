using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Names.ViewModels
{
    internal class SettingsViewModel: BaseViewModel
    {
        private bool _randomizeTimingCheckbox;
        public bool RandomizeTimingCheckbox
        {
            get => _randomizeTimingCheckbox;
            set => SetProperty(ref _randomizeTimingCheckbox, value);
        }
    }
}
