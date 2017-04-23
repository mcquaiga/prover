using System.Windows.Media;
using Action = System.Action;

namespace Prover.GUI.Common.Screens.MainMenu
{
    public interface IHaveMainMenuItem
    {
        ImageSource MenuIconSource { get; }
        string MenuTitle { get; }
        Action OpenAction { get; }
        int Order { get; }
    }
}