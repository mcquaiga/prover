namespace Prover.Application.Services
{
    //public class User
    //{
    //    public string EmployeeName { get; set; }
    //    public string EmployeeNbr { get; set; }
    //    public string Id { get; set; }
    //}

    //public class LocalLoginService<TUser> : LoginServiceBase<TUser>
    //    where TUser : class, new()
    //{
    //    private readonly Func<ICollection<TUser>, string, TUser> _lookupFunc;
    //    private readonly Func<TUser, string> _userIdLookupFunc;

    //    private readonly ICollection<TUser> _users;

    //    public LocalLoginService(ICollection<TUser> users, Func<ICollection<TUser>, string, TUser> lookupFunc,
    //        Func<TUser, string> userIdLookupFunc)
    //    {
    //        _lookupFunc = lookupFunc;
    //        _userIdLookupFunc = userIdLookupFunc;
    //        _users = users;
    //    }

    //    public override async Task<bool> Login(string username, string password = null)
    //    {
    //        await Task.Delay(TimeSpan.FromSeconds(1));

    //        User = _lookupFunc.Invoke(_users, username);

    //        LoggedInSubject.OnNext(User != null);

    //        return User != null;
    //    }

    //    public override async Task Logout()
    //    {
    //        await Task.Delay(TimeSpan.FromSeconds(1));
    //        await base.Logout();
    //    }

    //    protected override string UserId => _userIdLookupFunc.Invoke(User);
    //}
}