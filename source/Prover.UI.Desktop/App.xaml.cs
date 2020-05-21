using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Prover.UI.Desktop
{
	/// <summary>
	///     Interaction logic for App.xaml
	/// </summary>
	public partial class App : System.Windows.Application
	{
		public IHost AppHost { get; set; }

		public App()
		{
			_application = this;
			AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
		}

		private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			Console.WriteLine(args.LoadedAssembly);
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			// Select the text in a TextBox when it receives focus.
			EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseLeftButtonDownEvent,
				new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));

			EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotKeyboardFocusEvent,
				new RoutedEventHandler(SelectAllText));

			EventManager.RegisterClassHandler(typeof(TextBox), Control.MouseDoubleClickEvent,
				new RoutedEventHandler(SelectAllText));

			base.OnStartup(e);
		}


		void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
		{
			// Find the TextBox
			DependencyObject parent = e.OriginalSource as UIElement;
			while (parent != null && !(parent is TextBox))
				parent = VisualTreeHelper.GetParent(parent);

			if (parent != null)
			{
				var textBox = (TextBox)parent;
				if (!textBox.IsKeyboardFocusWithin)
				{
					// If the text box is not yet focused, give it the focus and
					// stop further processing of this click event.
					textBox.Focus();
					e.Handled = true;
				}
			}
		}

		void SelectAllText(object sender, RoutedEventArgs e)
		{
			var textBox = e.OriginalSource as TextBox;
			if (textBox != null)
				textBox.SelectAll();
		}
	}

	public partial class App
	{
		private static App _application;
		public static string Title => GetAppTitle();
		public static string VersionNumber { get; } = GetVersionNumber();

		private static string GetVersionNumber()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
			return fileVersionInfo.FileVersion;
		}

		private static string GetAppTitle()
		{

			return $"EVC Prover" +
				   $" - v{GetVersionNumber()}" +
				   $" - { _application.AppHost.Services.GetService<IHostEnvironment>().EnvironmentName }";
		}
	}
}