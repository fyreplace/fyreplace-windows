using CommunityToolkit.Mvvm.ComponentModel;
using Fyreplace.Events;
using Fyreplace.Extensions;
using Fyreplace.Services;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Windows.Input;

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

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowEditOverlay))]
        private bool editable;

        public ICommand? Command { get; set; }
        public event ClickHandler? Click;

        private bool hoverActive;
        private bool HoverActive
        {
            get => hoverActive;
            set
            {
                SetProperty(ref hoverActive, value);
                OnPropertyChanged(nameof(ShowEditOverlay));
            }
        }

        private CornerRadius Radius => new(Size / 2);
        private ValueWrapper<string>? AvatarWrapper => User != null && User.Avatar != string.Empty ? new(User.Avatar) : null;
        private bool IsTintApplied => Tinted && User != null;
        private SolidColorBrush Tint => new(User?.Tint.ToWindowsColor() ?? Colors.Transparent);
        private bool ShowEditOverlay => HoverActive && Editable;

        private readonly IEventBus eventBus = AppBase.GetService<IEventBus>();

        public Avatar()
        {
            InitializeComponent();
            eventBus.Subscribe<ModelChangedEvent>(OnModelChangedEventAsync);
        }

        #region Event Handlers

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e) => HoverActive = true;

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e) => HoverActive = false;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(sender, e);
            Command?.Execute(null);
            HoverActive = false;
        }

        private Task OnModelChangedEventAsync(ModelChangedEvent e)
        {
            if (e.Id != User?.Id)
            {
                return Task.CompletedTask;
            }

            switch (e.PropertyName)
            {
                case nameof(User.Avatar):
                    OnPropertyChanged(nameof(AvatarWrapper));
                    break;
            }

            return Task.CompletedTask;
        }

        #endregion

        private record class ValueWrapper<T>(T Value) { }
    }
}
