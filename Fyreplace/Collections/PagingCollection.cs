using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Fyreplace.Collections
{
    public sealed class PagingCollection<T>(Func<int?, Task<ICollection<T>>> fetch) : ObservableCollection<T>
    {
        private readonly Func<int?, Task<ICollection<T>>> fetch = fetch;
        private bool full;
        private int currentPage;

        public async Task<bool> FetchMoreAsync()
        {
            if (full)
            {
                return false;
            }

            var items = await fetch(currentPage);

            if (items.Count == 0)
            {
                full = true;
                return false;
            }

            currentPage++;

            foreach (var item in items)
            {
                Add(item);
            }

            return true;
        }
    }
}
