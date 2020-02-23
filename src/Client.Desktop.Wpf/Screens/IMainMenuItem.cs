﻿using System.Reactive;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace Client.Wpf.Screens
{
    public interface IMainMenuItem
    {
        PackIconKind MenuIconKind { get; }
        string MenuTitle { get; }

        ReactiveCommand<Unit, IRoutableViewModel> OpenCommand { get; }

        int? Order { get; }
    }
}