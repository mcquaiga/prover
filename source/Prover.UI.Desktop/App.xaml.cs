using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prover.UI.Desktop.Startup;
using Prover.UI.Desktop.ViewModels;
using Prover.UI.Desktop.Views;
using Control = System.Windows.Controls.Control;
using TextBox = System.Windows.Controls.TextBox;

namespace Prover.UI.Desktop {
	/// <summary>
	///     Interaction logic for App.xaml
	/// </summary>
	public partial class App : System.Windows.Application {
		private static StartScreen _splashScreen;

		private static readonly Func<AppBootstrapper, App> OnStartedCallback = boot => {
			var app = new App(boot);
			app.Started();
			return app;
		};

		private AppBootstrapper _appBootstrapper;

		//private static App Instance;

		public App() {

		}

		public App(AppBootstrapper appBootstrapper) {
			ShutdownMode = ShutdownMode.OnExplicitShutdown;
			AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

			_appBootstrapper = appBootstrapper;
			SetAppInfo();
		}

		public MainWindow AppMainWindow { get; private set; }
		public IHost AppHost => _appBootstrapper.AppHost;


		public static Func<AppBootstrapper, App> Starting() {
			ShowSplashScreen();
			return OnStartedCallback;
		}

		public static async Task<App> Starting(Func<Task<AppBootstrapper>> startup) {
			ShowSplashScreen();
			var boot = await startup();
			return new App(boot).Started();
		}

		protected override void OnStartup(StartupEventArgs e) {

			// Select the text in a TextBox when it receives focus.
			EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
			EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotKeyboardFocusEvent, new RoutedEventHandler(SelectAllText));
			EventManager.RegisterClassHandler(typeof(TextBox), Control.MouseDoubleClickEvent, new RoutedEventHandler(SelectAllText));

			base.OnStartup(e);
		}

		private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args) {
			Console.WriteLine(args.LoadedAssembly);
		}

		private MainWindow LoadMainWindow() {
			var model = AppHost.Services.GetService<MainViewModel>();
			var mainWindow = AppHost.Services.GetService<MainWindow>(); //ViewLocator.Current.ResolveView(model);
			mainWindow.ViewModel = model;

			//mainWindow.Closing += (sender, args) => { ExitApp(); };
			mainWindow.InitializeComponent();
			mainWindow.Show();
			return mainWindow;
		}

		private static void RegisterEventHandlers() {
			//Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>(
			//handler => {
			//	MouseButtonEventHandler (sender, e) => SelectivelyIgnoreMouseButton(sender, e);
			//}, 
			//handler => UIElement.PreviewMouseLeftButtonDownEvent += handler,
			//handler => KeyPress -= handler)
		}

		//public static event EventHandler<EventArgs> ExitRequested;
		//private static void OnExitRequested(EventArgs e) {
		//	ExitRequested?.Invoke(AppMainWindow, e);
		//}

		private static void SelectAllText(object sender, RoutedEventArgs e) {
			var textBox = e.OriginalSource as TextBox;

			if (textBox != null)
				textBox.SelectAll();
		}

		private static void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e) {
			// Find the TextBox
			DependencyObject parent = e.OriginalSource as UIElement;

			while (parent != null && !(parent is TextBox))
				parent = VisualTreeHelper.GetParent(parent);

			if (parent != null) {
				var textBox = (TextBox)parent;

				if (!textBox.IsKeyboardFocusWithin) {
					// If the text box is not yet focused, give it the focus and
					// stop further processing of this click event.
					textBox.Focus();
					e.Handled = true;
				}
			}
		}

		private static void ShowSplashScreen() {
			_splashScreen = new StartScreen();
			_splashScreen.Show();
		}

		private App Started() {
			InitializeComponent();
			AppMainWindow = LoadMainWindow();
			_splashScreen.Owner = AppMainWindow;
			_splashScreen.Close();
			return this;
		}
	}

	public partial class App {

		~App() {

			AppEnvironment = "Production";
		}
		public static string Title => "EVC Prover" + $" - v{VersionNumber}" + $" - {AppEnvironment}";
		public static string VersionNumber => GetVersionNumber();
		public static string AppEnvironment { get; private set; }

		private static string GetVersionNumber() {
			var assembly = Assembly.GetExecutingAssembly();
			var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
			return fileVersionInfo.FileVersion;
		}

		private void SetAppInfo() {
			AppEnvironment = AppHost.Services.GetService<IHostEnvironment>().EnvironmentName;

		}
	}
}