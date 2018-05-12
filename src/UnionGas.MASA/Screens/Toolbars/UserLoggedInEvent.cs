namespace UnionGas.MASA.Screens.Toolbars
{
    public class UserLoggedInEvent
    {
        public LogInState LoginStatus { get; }

        public enum LogInState
        {
            LoggedIn,
            LoggedOut
        }

        public UserLoggedInEvent(LogInState loginStatus)
        {
            LoginStatus = loginStatus;
        }
    }
}