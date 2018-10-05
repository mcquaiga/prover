using System;
using MaterialDesignThemes.Wpf;

namespace Prover.GUI.Screens.MainMenu
{
    public interface IHaveMainMenuItem
    {
        PackIconKind MenuIconKind { get; }
        string MenuTitle { get; }
        Action OpenAction { get; }
        int Order { get; }
    }
}