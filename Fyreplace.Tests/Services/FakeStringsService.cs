using Fyreplace.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fyreplace.Tests.Services
{
    public class FakeStringsService : IStringsService
    {
        public string GetString(string key) => key;
    }
}
