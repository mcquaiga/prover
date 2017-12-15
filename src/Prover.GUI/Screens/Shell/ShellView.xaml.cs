using System;
using System.Reactive.Linq;
using System.Windows;
using Prover.Core.Settings;
using ReactiveUI;

namespace Prover.GUI.Screens.Shell
{
    /// <summary>
    ///     Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : IViewFor<ShellViewModel>
    {
        public ShellView()
        {
            InitializeComponent();
            
            Style = (Style) FindResource(typeof(Window));

            this.WhenActivated(d =>
            {
                d(ViewModel = (ShellViewModel) DataContext);

                d(this.WhenAnyValue(x => x.ViewModel.GoHomeCommand)
                    .SelectMany(x => x.Execute())
                    .Subscribe());

                d(this.Bind(ViewModel, model => model.WindowHeight, view => view.Height));
                d(this.Bind(ViewModel, model => model.WindowWidth, view => view.Width));
            });
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ShellViewModel) value;
        }

        public ShellViewModel ViewModel { get; set; }
    }
}