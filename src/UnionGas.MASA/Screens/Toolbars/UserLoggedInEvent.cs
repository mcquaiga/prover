namespace UnionGas.MASA.Screens.Toolbars {
	public class UserLoggedInEvent {
		public LogInState LoginStatus { get; }

		public enum LogInState {
			LoggedIn,
			LoggedOut,
			InProgress
		}

		public UserLoggedInEvent(LogInState loginStatus) {
			LoginStatus = loginStatus;
		}

		public static UserLoggedInEvent Raise(LogInState state) {
			return new UserLoggedInEvent(state);
		}
	}
}