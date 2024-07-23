using Fyreplace.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Environment = Fyreplace.Data.Environment;

namespace Fyreplace.Pages
{
    public abstract class AccountPageBase : Page
    {
        protected Environment[] Environments
        {
            get
            {
                Environment[] value = [
                    Environment.Main,
                    Environment.Dev
                ];

#if DEBUG
                return [.. value, Environment.Local];
#else
                return value;
#endif
            }
        }

        protected IEnumerable<string> EnvironmentNames => Environments.Select(e => e.Description());

        protected int SelectedEnvironmentIndex
        {
            get => Array.IndexOf(Environments, settings.Environment);
            set => settings.Environment = Environments[value];
        }

        protected abstract IDictionary<string, UIElement> ConnectedElements { get; }

        private readonly ISettings settings = AppBase.GetService<ISettings>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var animator = ConnectedAnimationService.GetForCurrentView();

            foreach (var entry in ConnectedElements)
            {
                animator.GetAnimation(entry.Key)?.TryStart(entry.Value);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            var animator = ConnectedAnimationService.GetForCurrentView();

            foreach (var entry in ConnectedElements)
            {
                animator
                    .PrepareToAnimate(entry.Key, entry.Value)
                    .Configuration = new DirectConnectedAnimationConfiguration();
            }

            base.OnNavigatingFrom(e);
        }
    }
}
