using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fyreplace.Services
{
    public class StringsService : IStringsService
    {
        private readonly ResourceLoader resources = new();

        public string GetString(string key) => resources.GetString(key);
    }
}
