using System.ComponentModel;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Screens.Dialogs;
using MvvmDialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels
{
    public interface IScreenManager : IScreen
    {
        Task<IRoutableViewModel> ChangeView(IRoutableViewModel viewModel);
        Task<IRoutableViewModel> ChangeView<TViewModel>() where TViewModel : IRoutableViewModel;

        bool? ShowDialog<TViewModel>(ReactiveObject owner, IModalDialogViewModel viewModel)
            where TViewModel : IWindow;

        void ShowDialog(INotifyPropertyChanged viewModel);
        void ShowModalDialog(ReactiveObject owner, IModalDialogViewModel viewModel);

        DialogServiceManager DialogManager { get; set; }

    }
}