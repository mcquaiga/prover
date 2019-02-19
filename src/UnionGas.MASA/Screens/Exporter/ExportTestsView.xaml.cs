namespace UnionGas.MASA.Screens.Exporter
{
    using ReactiveUI;
    using System;
    using System.Reactive.Linq;

    /// <summary>
    /// Defines the <see cref="ExportTestsView" />
    /// </summary>
    public partial class ExportTestsView : IViewFor<ExportTestsViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportTestsView"/> class.
        /// </summary>
        public ExportTestsView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                ViewModel = (ExportTestsViewModel)DataContext;

                d(this.WhenAnyValue(x => x.ViewModel.ExecuteTestSearch)
                    .SelectMany(x => x.Execute())
                    .Subscribe());
            });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ViewModel
        /// </summary>
        public ExportTestsViewModel ViewModel { get; set; }

        /// <summary>
        /// Gets or sets the ViewModel
        /// </summary>
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ExportTestsViewModel)value; }
        }

        #endregion
    }
}
