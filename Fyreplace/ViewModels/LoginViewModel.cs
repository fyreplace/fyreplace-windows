﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace Fyreplace.ViewModels
{
    public sealed partial class LoginViewModel : AccountViewModelBase
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSubmit))]
        public string identifier = "";

        public bool CanSubmit => !string.IsNullOrWhiteSpace(Identifier)
            && Identifier.Length >= 3
            && Identifier.Length <= 254;
    }
}
