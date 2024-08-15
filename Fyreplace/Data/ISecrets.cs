using System.ComponentModel;

namespace Fyreplace.Data
{
    public interface ISecrets : INotifyPropertyChanged
    {
        public string Token { get; set; }
    }
}
