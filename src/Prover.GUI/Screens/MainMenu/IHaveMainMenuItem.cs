using System;
using System.Windows.Media;

namespace Prover.GUI.Screens.MainMenu
{
    public interface IHaveMainMenuItem
    {
        ImageSource MenuIconSource { get; }
        string MenuTitle { get; }
        Action OpenAction { get; }
        int Order { get; }
    }
}