using System;
using System.Collections.Generic;
using System.Reactive;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Prover.Shared.Extensions;
using ReactiveUI;

namespace Prover.UI.Desktop.Controls
{
    /// <summary>
    ///     Interaction logic for DateFilterControl.xaml
    /// </summary>
    public partial class DateFilterControl : UserControl
    {
        private static readonly string _defaultFilter = "7d";

        //public static readonly Dictionary<string, Func<DateTime, bool>> _dateFilters = new Dictionary<string, Func<DateTime, bool>>
        //{
        //        {"1h", time => time.IsLessThanTimeAgo(TimeSpan.FromHours(1))},
        //        {"1d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(1))},
        //        {"3d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(3))},
        //        {"7d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(7))},
        //        {"30d", time => time.IsLessThanTimeAgo(TimeSpan.FromDays(30))}
        //};

        public static readonly DependencyProperty DefaultDateProperty = DependencyProperty.Register("DefaultDate",
        typeof(string),
        typeof(DateFilterControl),
        new PropertyMetadata(_defaultFilter));

        public static readonly DependencyProperty DateFiltersProperty = DependencyProperty.Register("DateFilters",
        typeof(ICollection<string>),
        typeof(DateFilterControl),
        new PropertyMetadata(default(ICollection<string>)));

        public static readonly DependencyProperty ApplyFilterCommandProperty = DependencyProperty.Register("ApplyFilterCommand",
        typeof(ICommand),
        typeof(DateFilterControl),
        new PropertyMetadata(default(ICommand)));

        public DateFilterControl()
        {
            InitializeComponent();
            //DateFilters = _dateFilters;
        }

        public string DefaultDate
        {
            get => (string) GetValue(DefaultDateProperty);
            set => SetValue(DefaultDateProperty,
            value);
        }

        public ICollection<string> DateFilters
        {
            get => (ICollection<string>) GetValue(DateFiltersProperty);
            set => SetValue(DateFiltersProperty,
            value);
        }

        public ICommand ApplyFilterCommand
        {
            get => (ICommand) GetValue(ApplyFilterCommandProperty);
            set => SetValue(ApplyFilterCommandProperty,
            value);
        }

        public static readonly DependencyProperty FilterCommandParameterProperty = DependencyProperty.Register("FilterCommandParameter",
        typeof(object),
        typeof(DateFilterControl),
        new PropertyMetadata(default(object)));

        public object FilterCommandParameter
        {
            get { return (object) GetValue(FilterCommandParameterProperty); }
            set
            {
                SetValue(FilterCommandParameterProperty,
                value);
            }
        }
    }
}