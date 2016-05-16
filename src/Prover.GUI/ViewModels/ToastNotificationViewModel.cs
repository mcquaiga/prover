using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.GUI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.GUI.ViewModels
{
    public class ToastNotificationViewModel : ReactiveScreen, IHandle<NotificationEvent>
    {
        private IUnityContainer _container;

        public ToastNotificationViewModel(IUnityContainer container)
        {
            _container = container;
        }

        public string Message { get; set; }

        public void Handle(NotificationEvent message)
        {
            Message = message.Message;
            NotifyOfPropertyChange(() => Message);
        }
    }
}
