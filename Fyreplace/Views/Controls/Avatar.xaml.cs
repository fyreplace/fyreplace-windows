using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Fyreplace.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Fyreplace.Extensions;
using Microsoft.UI.Xaml.Media;

namespace Fyreplace.Views.Controls
{
    [ObservableObject]
    public sealed partial class Avatar : UserControl
    {
        public delegate void ClickHandler(object sender, RoutedEventArgs e);

        [ObservableProperty]
        private int size;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AvatarWrapper))]
        [NotifyPropertyChangedFor(nameof(IsTintApplied))]
        [NotifyPropertyChangedFor(nameof(Tint))]
        private User? user;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsTintApplied))]
        private bool tinted;

        private ValueWrapper<string>? AvatarWrapper => User != null && User.Avatar != string.Empty ? new(User.Avatar) : null;

        public event ClickHandler? Click;

        private CornerRadius Radius => new(Size / 2);

        private bool IsTintApplied => Tinted && User != null;

        private SolidColorBrush Tint => new(User?.Tint.ToWindowsColor() ?? Colors.Transparent);

        public Avatar() => InitializeComponent();

        private void Button_Click(object sender, RoutedEventArgs e) => Click?.Invoke(sender, e);

        private record class ValueWrapper<T>(T Value) { }
    }
}
