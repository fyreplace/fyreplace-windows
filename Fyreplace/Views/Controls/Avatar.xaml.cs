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

        public ICommand? Command { get; set; }

        private CornerRadius Radius => new(Size / 2);
        private ValueWrapper<string>? AvatarWrapper => User != null && !string.IsNullOrEmpty(User.Avatar) ? new(User.Avatar) : null;
        private bool IsTintApplied => User != null;
        private SolidColorBrush Tint => new(User?.Tint.ToWindowsColor() ?? Colors.Transparent);

        private readonly IEventBus eventBus = AppBase.GetService<IEventBus>();

        public Avatar()
        {
            InitializeComponent();
            eventBus.Subscribe<ModelChangedEvent>(OnModelChangedEventAsync);
        }

        #region Event Handlers

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
