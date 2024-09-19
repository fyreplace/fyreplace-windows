using Fyreplace.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fyreplace.Tests.Events
{
    public sealed class StoringEventBus : IEventBus
    {
        public IReadOnlyList<IEvent> Events => events.AsReadOnly();

        private readonly EventBus realBus = new();
        private readonly IList<IEvent> events = [];

        public void Subscribe<T>(object? target, Func<T, Task> handler) where T : IEvent => realBus.Subscribe(target, handler);

        public void Unsubscribe<T>(Func<T, Task> handler) where T : IEvent => realBus.Unsubscribe(handler);

        public Task PublishAsync(IEvent someEvent)
        {
            events.Add(someEvent);
            return realBus.PublishAsync(someEvent);
        }

        public void Clear() => events.Clear();
    }
}
