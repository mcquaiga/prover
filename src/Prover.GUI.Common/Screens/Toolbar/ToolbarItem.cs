using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.GUI.Common.Screens.MainMenu;
using Action = System.Action;

namespace Prover.GUI.Common.Screens.Toolbar
{
    public interface IToolbarItem
    {
        string ViewContext { get; }
    }
}
