using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.CompilerServices;

namespace Fyreplace.Data
{
    public abstract class DataStoreBase<K> : ObservableObject
    {
        public abstract K MakeCleanKey(string key);

        public K MakeKey([CallerMemberName] string? propertyName = null) => MakeCleanKey(propertyName!);
    }
}
