using Prover.Application.Extensions;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    /// <summary>
    /// Interaction logic for VerificationsGrid.xaml
    /// </summary>
    public partial class VerificationsGrid : IDisposable
    {
        public VerificationsGrid()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PrintDataTemplateProperty = DependencyProperty.Register(
            "PrintDataTemplate", typeof(DataTemplate), typeof(VerificationsGrid), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate PrintDataTemplate
        {
            get => (DataTemplate)GetValue(PrintDataTemplateProperty);
            set => SetValue(PrintDataTemplateProperty, value);
        }

        public static readonly DependencyProperty ToolbarViewModelProperty = DependencyProperty.Register("ToolbarViewModel", typeof(ExportToolbarViewModel), typeof(VerificationsGrid), new PropertyMetadata(default(ExportToolbarViewModel)));

        public ExportToolbarViewModel ToolbarViewModel
        {
            get { return (ExportToolbarViewModel)GetValue(ToolbarViewModelProperty); }
            set { SetValue(ToolbarViewModelProperty, value); }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            BindingOperations.ClearAllBindings(this);
        }

        private void VerificationsDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToolbarViewModel.PrintReport
                            .Execute(ToolbarViewModel.Selected)
                            .LogErrors()
                            .Subscribe();
        }
    }
}
