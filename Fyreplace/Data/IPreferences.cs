using System.ComponentModel;

namespace Fyreplace.Data
{
    public interface IPreferences : INotifyPropertyChanged
    {
        public Environment Connection_Environment { get; set; }

        public string Account_Identifier { get; set; }

        public string Account_Username { get; set; }

        public string Account_Email { get; set; }

        public bool Account_IsWaitingForRandomCode { get; set; }
    }
}
