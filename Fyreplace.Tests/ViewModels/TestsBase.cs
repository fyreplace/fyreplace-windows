using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Tests.Data.Preferences;
using Fyreplace.Tests.Data.Secrets;
using Fyreplace.Tests.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fyreplace.Tests.ViewModels
{
    public abstract class TestsBase
    {
        [TestInitialize]
        public virtual Task TestInitialize()
        {
            GetEventBus().Clear();
            GetPreferences().Clear();
            GetSecrets().Clear();
            return Task.CompletedTask;
        }

        protected static StoringEventBus GetEventBus() => (StoringEventBus)AppBase.GetService<IEventBus>();
        protected static MemoryPreferences GetPreferences() => (MemoryPreferences)AppBase.GetService<IPreferences>();
        protected static MemorySecrets GetSecrets() => (MemorySecrets)AppBase.GetService<ISecrets>();
    }
}
