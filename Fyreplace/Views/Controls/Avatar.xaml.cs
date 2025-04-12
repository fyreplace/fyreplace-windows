using Fyreplace.Services;
using Fyreplace.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

namespace Fyreplace.Views.Controls
{
    public sealed partial class Avatar : UserControl
    {
        public delegate void ClickHandler(object sender, RoutedEventArgs e);

        public int Size {
            get { return viewModel.Size; }
            set { viewModel.Size = value; }
        }

        public User? User {
            get { return viewModel.User; }
            set { viewModel.User = value; }
        }

        public ICommand? Command {
            get { return viewModel.Command; }
            set { viewModel.Command = value; }
        }

        private readonly AvatarViewModel viewModel = AppBase.GetService<AvatarViewModel>();

        public Avatar() => InitializeComponent();
    }
}
