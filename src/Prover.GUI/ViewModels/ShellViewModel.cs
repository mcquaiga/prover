using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.GUI.Events;
using Prover.GUI.Interfaces;
using Prover.GUI.Views;
using ReactiveUI;
using System.Timers;

namespace Prover.GUI.ViewModels
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive, IShell, IHandle<ScreenChangeEvent>, IHandle<NotificationEvent>
    {
        private readonly IUnityContainer _container;
        private System.Timers.Timer _timer;
        private string _applicationEventMessage;

        public ShellViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            ActivateItem(new MainMenuViewModel(_container));
        }

        public void HomeButton()
        {
            ShowMainMenu();
        }

        public string ApplicationEventMessage
        {
            get { return _applicationEventMessage; }
            set
            {
                _applicationEventMessage = value;
                NotifyOfPropertyChange(() => ApplicationEventMessage);
            }
        }

        public string Title
        {
            get { return "Prover"; }
        }

        public void Handle(ScreenChangeEvent message)
        {
            ActivateItem(message.ViewModel);
        }

        public void Handle(NotificationEvent message)
        {
            _timer = new System.Timers.Timer(3000);
            ApplicationEventMessage = message.Message;
            _timer.Elapsed += OnApplicationEventMessage;
            _timer.Enabled = true;
        }

        private void OnApplicationEventMessage(object sender, ElapsedEventArgs e)
        {
            ApplicationEventMessage = "";
        }
    }
}
