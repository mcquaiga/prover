using MaterialDesignThemes.Wpf;
using Action = System.Action;

namespace Prover.GUI.Common.Screens.MainMenu
{
    public interface IAppMainMenu
    {
        PackIconKind IconKind { get; }
        string AppTitle { get; }
        Action ClickAction { get; }
    }
}