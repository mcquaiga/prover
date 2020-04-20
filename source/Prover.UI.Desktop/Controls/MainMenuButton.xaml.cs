using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Prover.UI.Desktop.Controls
{
    /// <summary>
    ///     Interaction logic for MainMenuButton.xaml
    /// </summary>
    public partial class MainMenuButton : UserControl
    {
        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
            "IconSource", typeof(ImageSource), typeof(MainMenuButton), new PropertyMetadata(default(ImageSource)));

        public static readonly DependencyProperty AppTitleProperty = DependencyProperty.Register(
            "AppTitle", typeof(string), typeof(MainMenuButton), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ClickActionProperty = DependencyProperty.Register(
            "ClickAction", typeof(Action), typeof(MainMenuButton), new PropertyMetadata(default(Action)));

        public MainMenuButton()
        {
            InitializeComponent();

            //this.WhenActivated(disposables =>
            //{
            //    this.BindCommand(ViewModel, x => x., x => x.MenuButton);

            //    // Bind the view model router to RoutedViewHost.Router property.
            //    //this.BindCommand(ViewModel, x => x.OpenAction, x => x.MenuButton)
            //    //    .DisposeWith(disposables);
            //});
        }

        public ImageSource IconSource
        {
            get => (ImageSource) ((DependencyObject) this).GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public string AppTitle
        {
            get => (string) ((DependencyObject) this).GetValue(AppTitleProperty);
            set => SetValue(AppTitleProperty, value);
        }

        public Action ClickAction
        {
            get => (Action) ((DependencyObject) this).GetValue(ClickActionProperty);
            set => SetValue(ClickActionProperty, value);
        }

        public string ButtonName => $"{AppTitle}Button";

        public void ActionCommand()
        {
            Task.Run(() => ClickAction);
        }
    }
}