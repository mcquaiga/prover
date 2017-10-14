using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Prover.GUI.Screens;


namespace Prover.GUI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {      
        public App()
        {
            InitializeComponent();      
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //works for tab into textbox
            EventManager.RegisterClassHandler(typeof(TextBox),
                TextBox.GotFocusEvent,
                new RoutedEventHandler(TextBox_GotFocus));  

            base.OnStartup(e);
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }        
    }
}