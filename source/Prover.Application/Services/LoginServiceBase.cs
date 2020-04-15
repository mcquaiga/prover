using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using Prover.Shared.Interfaces;
using ReactiveUI;

namespace Prover.Application.Services
{
    public abstract class LoginServiceBase<TUser> : ILoginService<TUser>, IDisposable
            where TUser : IUser, new()
    {
        private readonly CompositeDisposable _cleanup;
        protected readonly ISubject<bool> LoggedInSubject = new Subject<bool>();

        protected LoginServiceBase()
        {
            var loggedIn = LoggedInSubject.ObserveOn(RxApp.MainThreadScheduler).Publish();

            LoggedIn = loggedIn;

            LoggedIn.Subscribe(x => IsSignedOn = x);

            _cleanup = new CompositeDisposable(loggedIn.Connect());

            LoggedInSubject.OnNext(false);
        }

        public TUser User { get; protected set; }
        public IObservable<bool> LoggedIn { get; }
        public bool IsSignedOn { get; private set; }

        protected abstract string UserId { get; }


        public void Dispose()
        {
            User = default;
            _cleanup?.Dispose();
        }

        public virtual async Task<bool> Execute(EvcVerificationViewModel verification)
        {
            await Task.CompletedTask;

            LoggedIn.Subscribe(x => { verification.EmployeeId = UserId; }).DisposeWith(_cleanup);

            return true;
        }

        /// <inheritdoc />
        public abstract Task<string> GetDisplayName<TId>(TId id);

        public virtual async Task<string> GetLoginDetails() => await MessageInteractions.GetInputString.Handle("Username:");

        /// <inheritdoc />
        /// <inheritdoc />
        public IUser GetSignOnUser() => User;

        /// <inheritdoc />
        public abstract Task<TUser> GetUserDetails<TId>(TId id);

        public abstract IEnumerable<TUser> GetUsers();


        public abstract Task<bool> Login(string username, string password = null);

        public async Task<bool> Login()
        {
            await GetLoginDetails()
                    .ContinueWith(task => { Login(task.Result); });

            return await Task.FromResult(true);
        }

        public virtual async Task Logout()
        {
            User = default;
            LoggedInSubject.OnNext(User != null);
            await Task.CompletedTask;
        }

        public IObservable<bool> SignOn()
        {
            return Observable.StartAsync(async () =>
            {
                var username = await GetLoginDetails();
                return await Login(username);
            });
        }
    }
}