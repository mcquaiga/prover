using System.Reactive;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Prover.Application.ViewModels
{
	public abstract class RoutableViewModelBase : ViewModelBase, IRoutableViewModel
	{
		protected RoutableViewModelBase(IScreenManager screenManager, ILogger<RoutableViewModelBase> logger = null, string urlPathSegment = null)
			: base(logger)
		{
			ScreenManager = screenManager;
			HostScreen = screenManager;
			UrlPathSegment = urlPathSegment;

			CloseView = ReactiveCommand.CreateFromTask(ScreenManager.GoBack);


		}

		public IScreenManager ScreenManager { get; protected set; }
		public ReactiveCommand<Unit, Unit> CloseView { get; protected set; }
		public string UrlPathSegment { get; protected set; }
		public IScreen HostScreen { get; protected set; }


	}
}