using Prover.Application.Interactions;
using Prover.Application.ViewModels;
using Prover.Modules.UnionGas.Models;
using Prover.Shared.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Prover.Application.Interfaces;
using Prover.UI.Desktop.Controls;

namespace Prover.Modules.UnionGas.Login
{
	public class LoginToolbarViewModel : ViewModelBase, IModuleToolbarItem
	{
		public LoginToolbarViewModel(ILoginService<Employee> loginService)
		{
			LoginService = loginService;
			LogIn = ReactiveCommand.CreateFromTask(async () =>
			{
				var username = await loginService.GetLoginDetails();
				return await loginService.Login(username);
			},
			canExecute: LoginService.LoggedIn.Select(x => !x),
			outputScheduler: RxApp.MainThreadScheduler);

			LogIn.ThrownExceptions
				.ObserveOn(RxApp.MainThreadScheduler)
				.Do(async ex => await Exceptions.Error.Handle($"An error occured signing on. {Environment.NewLine} {ex.Message}"))
				.Subscribe().DisposeWith(Cleanup);

			LogIn.Where(x => !x)
				.Do(async _ => await Notifications.SnackBarMessage.Handle("Employee not found"))
				.Subscribe().DisposeWith(Cleanup);

			LogOut = ReactiveCommand.CreateFromTask(loginService.Logout);

			loginService.LoggedIn
				.Select(x => x ? loginService.User?.UserName : "")
				.ObserveOn(RxApp.MainThreadScheduler)
				.ToPropertyEx(this, vm => vm.DisplayName);
		}

		public ILoginService<Employee> LoginService { get; }

		public extern string DisplayName { [ObservableAsProperty] get; }

		public ReactiveCommand<Unit, bool> LogIn { get; }

		public ReactiveCommand<Unit, Unit> LogOut { get; }

		/// <inheritdoc />
		public int SortOrder { get; } = 1;
	}
}