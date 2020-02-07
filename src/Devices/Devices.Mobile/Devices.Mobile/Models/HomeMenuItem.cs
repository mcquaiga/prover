using System;
using System.Collections.Generic;
using System.Text;

namespace Devices.Mobile.Models
{
    public enum MenuItemType
    {
        Devices,
        Ports
    }

    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}