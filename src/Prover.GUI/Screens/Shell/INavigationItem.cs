using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace Prover.GUI.Screens.Shell
{
    public interface INavigationItem
    {
        ReactiveCommand<Unit, Unit> NavigationCommand { get; }
        PackIconKind IconKind { get; }
        bool IsHome { get; }
    }
}
