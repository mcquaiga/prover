using Prover.Application.ViewModels;
using Prover.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Prover.Application.Services
{
    public abstract class LoginServiceBase<TUser> : ILoginService<TUser>, IDisposable
            where TUser : IUser, new()
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        protected readonly ISubject<bool> LoggedInSubject = new Subject<bool>();

        protected LoginServiceBase()
        {
            LoggedIn = LoggedInSubject.StartWith(false);

            LoggedIn.Subscribe(x => IsSignedOn = x).DisposeWith(_cleanup);

            //LoggedInSubject.OnNext(false);
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

            LoggedIn.Subscribe(x => { verification.EmployeeId = UserId; })
                    .DisposeWith(_cleanup);

            return true;
        }

        /// <inheritdoc />
        public abstract Task<string> GetDisplayName<TId>(TId id);

        public virtual async Task<string> GetLoginDetails() => await Interactions.Messages.GetInputString.Handle("Username:");

        /// <inheritdoc />
        /// <inheritdoc />
        public IUser GetSignOnUser() => User;

        /// <inheritdoc />
        public abstract Task<TUser> GetUserDetails<TId>(TId id);

        public abstract IEnumerable<TUser> GetUsers();


        public abstract Task<bool> Login(string username, string password = null);

        public async Task<bool> Login()
        {
            var username = await GetLoginDetails();
            var success = await Login(username);
            LoggedInSubject.OnNext(success);
            return await Task.FromResult(success);
        }

        public virtual async Task Logout()
        {
            User = default;
            LoggedInSubject.OnNext(User != null);
            await Task.CompletedTask;
        }

        public IObservable<bool> SignOn()
        {
            return Observable.FromAsync(async () =>
            {
                var username = await GetLoginDetails();
                return await Login(username);
            });
        }
    }
}