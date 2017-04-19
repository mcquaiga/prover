using System;
using System.Windows.Controls;
using Prover.ProtocolMonitor.ViewModel;
using ReactiveUI;

namespace Prover.ProtocolMonitor.View
{
    /// <summary>
    ///     Interaction logic for Shell.xaml
    /// </summary>
    public partial class SerialCommView : UserControl, IViewFor<SerialCommViewModel>
    {
        public SerialCommView()
        {
            ViewModel = new SerialCommViewModel();
            InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (SerialCommViewModel) value; }
        }

        public SerialCommViewModel ViewModel { get; set; }
    }
}