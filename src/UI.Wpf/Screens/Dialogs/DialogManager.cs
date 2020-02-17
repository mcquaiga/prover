using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Client.Wpf.ViewModels;
using Client.Wpf.ViewModels.Devices;
using Client.Wpf.Views.Devices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MvvmDialogs;
using MvvmDialogs.DialogFactories;
using MvvmDialogs.DialogTypeLocators;
using MvvmDialogs.FrameworkDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using MvvmDialogs.FrameworkDialogs.MessageBox;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Wpf.Screens.Dialogs
{
    public class DialogGuy : ReactiveObject
    {
        private readonly IServiceProvider _services;
        private readonly IDialogFactory dialogFactory;
        private readonly IDialogTypeLocator dialogTypeLocator;
        private readonly IFrameworkDialogFactory frameworkDialogFactory;
        private IObservable<UserControl> _dialogView;

        public DialogGuy(
            IServiceProvider services,
            IDialogFactory? dialogFactory = null,
            IDialogTypeLocator? dialogTypeLocator = null,
            IFrameworkDialogFactory? frameworkDialogFactory = null)
        {
            _services = services;
            this.dialogFactory = dialogFactory ?? new ReflectionDialogFactory();
            this.dialogTypeLocator = dialogTypeLocator ?? new NamingConventionDialogTypeLocator();
            this.frameworkDialogFactory = frameworkDialogFactory ?? new DefaultFrameworkDialogFactory();

        }

        [Reactive] 
        public ReactiveObject DialogViewModel { get; set; }
        
        [Reactive] 
        public UserControl DialogView { get; set; }

        //public IObservable<UserControl> DialogViewObservable => _dialogView;

        public void Show<T>(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel)
            where T : UserControl
        {
            if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));


            DialogViewModel = (ReactiveObject)viewModel;

            DialogView = _services.GetService<T>();
            (DialogView as IViewFor<ReactiveObject>).ViewModel = DialogViewModel;
            //DialogView = CreateDialog<T>(ownerViewModel, viewModel);
            //Show<T>(ownerViewModel, viewModel);
        }

        public void Show(SessionDialogView view, SessionDialogViewModel viewModel)
        {

            DialogView = view;
            DialogViewModel = viewModel;
        }
    }



    public class DialogManager : ReactiveObject, IDialogService
    {
        private readonly IServiceProvider _services;
        private readonly IDialogFactory dialogFactory;
        private readonly IDialogTypeLocator dialogTypeLocator;
        private readonly IFrameworkDialogFactory frameworkDialogFactory;

        public DialogManager(
            IServiceProvider services,
            IDialogFactory? dialogFactory = null,
            IDialogTypeLocator? dialogTypeLocator = null,
            IFrameworkDialogFactory? frameworkDialogFactory = null)
        {
            _services = services;
            this.dialogFactory = dialogFactory ?? new ReflectionDialogFactory();
            this.dialogTypeLocator = dialogTypeLocator ?? new NamingConventionDialogTypeLocator();
            this.frameworkDialogFactory = frameworkDialogFactory ?? new DefaultFrameworkDialogFactory();
        }

        [Reactive] 
        public ReactiveObject DialogViewModel { get; set; }
        
        [Reactive] 
        public IViewFor<ReactiveObject> DialogView { get; set; }

        public void Show<T>(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel) where T : Window
        {
            if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));


            DialogViewModel = (ReactiveObject)viewModel;

            DialogView = (IViewFor<ReactiveObject>)_services.GetService<T>();
            DialogView.ViewModel = DialogViewModel;
            //DialogView = CreateDialog<T>(ownerViewModel, viewModel);

            
            
            //Show<T>(ownerViewModel, viewModel);
        }

        public void ShowCustom<T>(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel) where T : IWindow
        {
            throw new NotImplementedException();
        }

        public void Show(INotifyPropertyChanged ownerViewModel, INotifyPropertyChanged viewModel)
        {
            throw new NotImplementedException();
        }

        public bool? ShowDialog<T>(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel) where T : Window => throw new NotImplementedException();

        public bool? ShowCustomDialog<T>(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel) where T : IWindow => throw new NotImplementedException();

        public bool? ShowDialog(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel) => throw new NotImplementedException();

        public bool Activate(INotifyPropertyChanged viewModel) => throw new NotImplementedException();

        public MessageBoxResult ShowMessageBox(INotifyPropertyChanged ownerViewModel, string messageBoxText, string caption = "",
            MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, MessageBoxResult defaultResult = MessageBoxResult.None) => throw new NotImplementedException();

        public MessageBoxResult ShowMessageBox(INotifyPropertyChanged ownerViewModel, MessageBoxSettings settings) => throw new NotImplementedException();

        public bool? ShowOpenFileDialog(INotifyPropertyChanged ownerViewModel, OpenFileDialogSettings settings) => throw new NotImplementedException();

        public bool? ShowSaveFileDialog(INotifyPropertyChanged ownerViewModel, SaveFileDialogSettings settings) => throw new NotImplementedException();

        public bool? ShowFolderBrowserDialog(INotifyPropertyChanged ownerViewModel, FolderBrowserDialogSettings settings) => throw new NotImplementedException();

        //private void Show(
        //    INotifyPropertyChanged ownerViewModel,
        //    INotifyPropertyChanged viewModel,
        //    Type dialogType)
        //{
        //    //Logger<.Write($"Dialog: {dialogType}; View model: {viewModel.GetType()}; Owner: {ownerViewModel.GetType()}");
        //    DialogView = CreateDialog(dialogType, ownerViewModel, viewModel);
        //    DialogViewModel = (ReactiveObject)viewModel;
        //    //DialogView = CreateDialog(dialogType, ownerViewModel, viewModel);

        //}

        //private void Show<T>(
        //    INotifyPropertyChanged ownerViewModel,
        //    INotifyPropertyChanged viewModel) where T : ReactiveDialog<ReactiveObject>
        //{
        //    //Logger<.Write($"Dialog: {dialogType}; View model: {viewModel.GetType()}; Owner: {ownerViewModel.GetType()}");
        //    DialogView = CreateDialog<T>(ownerViewModel, viewModel);
        //    DialogViewModel = (ReactiveObject)viewModel;
        //    //DialogView = CreateDialog(dialogType, ownerViewModel, viewModel);

        //}

        private IWindow CreateDialog(
            Type dialogType,
            INotifyPropertyChanged ownerViewModel,
            INotifyPropertyChanged viewModel)
        {
            var dialog = _services.GetService(dialogType);
            //dialog.Owner = FindOwnerWindow(ownerViewModel);
            (dialog as IWindow).DataContext = (ReactiveObject)viewModel;

            return  (IWindow) dialog;
        }

        private T CreateDialog<T>(
            INotifyPropertyChanged ownerViewModel,
            INotifyPropertyChanged viewModel) where T : Window
        {
            var dialog = _services.GetService<T>();
            //dialog.Owner = FindOwnerWindow(ownerViewModel);
            (dialog as IWindow).DataContext = (ReactiveObject)viewModel;

            return  (T) dialog;
        }

        //private static Window FindOwnerWindow(INotifyPropertyChanged viewModel)
        //{
        //    IView view = DialogServiceViews.Views.SingleOrDefault(
        //        registeredView =>
        //            registeredView.Source.IsLoaded &&
        //            ReferenceEquals(registeredView.DataContext, viewModel));

        //    if (view == null)
        //    {
        //        string message = $"View model of type '{viewModel.GetType()}' is not present as data context on any registered view." +
        //                         "Please register the view by setting DialogServiceViews.IsRegistered=\"True\" in your XAML.";

        //        throw new ViewNotRegisteredException(message);
        //    }

        //    // Get owner window
        //    Window owner = view.GetOwner();
        //    if (owner == null) throw new InvalidOperationException($"View of type '{view.GetType()}' is not registered.");

        //    return owner;
        //}

    }
}
