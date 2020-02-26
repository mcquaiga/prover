using System.ComponentModel;
using System.Threading.Tasks;
using MvvmDialogs;
using ReactiveUI;

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
    }
}