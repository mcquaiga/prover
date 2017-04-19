using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Prover.Client.Framework.Controls
{
    /// <summary>
    ///     Interaction logic for MainMenuButton.xaml
    /// </summary>
    public partial class MainMenuButton : UserControl
    {
        public static readonly DependencyProperty AppTitleProperty = DependencyProperty.Register(
            "AppTitle", typeof(string), typeof(MainMenuButton), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ClickActionProperty = DependencyProperty.Register(
            "ClickAction", typeof(Action), typeof(MainMenuButton), new PropertyMetadata(default(Action)));

        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
            "IconSource", typeof(ImageSource), typeof(MainMenuButton), new PropertyMetadata(default(ImageSource)));

        public MainMenuButton()
        {
            InitializeComponent();
        }

        public string AppTitle
        {
            get { return (string) GetValue(AppTitleProperty); }
            set { SetValue(AppTitleProperty, value); }
        }

        public string ButtonName => $"{AppTitle}Button";

        public Action ClickAction
        {
            get { return (Action) GetValue(ClickActionProperty); }
            set { SetValue(ClickActionProperty, value); }
        }

        public ImageSource IconSource
        {
            get { return (ImageSource) GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        public void ActionCommand()
        {
            Task.Run(() => ClickAction);
        }
    }
}