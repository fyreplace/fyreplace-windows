using System;
using System.Threading.Tasks;

namespace Fyreplace.Events
{
    public interface IEventBus
    {
        public void Subscribe<T>(Func<T, Task> handler) where T : IEvent => Subscribe(handler.Target, handler);

        public void Subscribe<T>(object? target, Func<T, Task> handler) where T : IEvent;

        public void Unsubscribe<T>(Func<T, Task> handler) where T : IEvent;

        public Task Publish(IEvent someEvent);
    }
}
