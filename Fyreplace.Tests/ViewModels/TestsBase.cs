using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Tests.Data.Preferences;
using Fyreplace.Tests.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fyreplace.Tests.ViewModels
{
    public abstract class TestsBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            GetEventBus().Clear();
            GetPreferences().Clear();
        }

        protected static StoringEventBus GetEventBus() => (StoringEventBus)AppBase.GetService<IEventBus>();
        protected static MemoryPreferences GetPreferences() => (MemoryPreferences)AppBase.GetService<IPreferences>();
    }
}
