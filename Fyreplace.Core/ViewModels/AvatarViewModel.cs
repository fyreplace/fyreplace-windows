using CommunityToolkit.Mvvm.ComponentModel;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Extensions;
using Fyreplace.Services;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fyreplace.ViewModels
{
    public sealed partial class AvatarViewModel : ViewModelBase
    {
        [ObservableProperty]
        public partial int Size { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AvatarWrapper))]
        [NotifyPropertyChangedFor(nameof(IsTintApplied))]
        [NotifyPropertyChangedFor(nameof(Tint))]
        public partial User? User { get; set; }

        public ICommand? Command { get; set; }

        public CornerRadius Radius => new(Size / 2);
        public ValueWrapper<string>? AvatarWrapper => User != null && !string.IsNullOrEmpty(User.Avatar) ? new(User.Avatar) : null;
        public bool IsTintApplied => User != null;
        public SolidColorBrush Tint => new(User?.Tint.ToWindowsColor() ?? Colors.Transparent);

        public AvatarViewModel(IPreferences preferences, ISecrets secrets, IEventBus eventBus, IApiClient api)
            : base(preferences, secrets, eventBus, api)
        {
            eventBus.Subscribe<ModelChangedEvent>(OnModelChangedEventAsync);
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

        public record class ValueWrapper<T>(T Value) { }
    }
}
