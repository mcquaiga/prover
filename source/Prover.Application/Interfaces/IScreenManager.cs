using System.Threading.Tasks;
using ReactiveUI;

namespace Prover.Application.Interfaces
{
    public interface IScreenManager : IScreen
    {
        Task<TViewModel> ChangeView<TViewModel>(TViewModel viewModel) where TViewModel : IRoutableViewModel;
        Task<TViewModel> ChangeView<TViewModel>() where TViewModel : IRoutableViewModel;

        IDialogServiceManager DialogManager { get; }

        Task GoHome();
        Task GoBack();
    }
}