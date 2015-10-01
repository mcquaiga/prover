using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.GUI.Events
{
    public class NotificationEvent
    {
        public string Message { get; set; }
        public NotificationEvent(string message)
        {
            Message = message;
        }
    }
}
