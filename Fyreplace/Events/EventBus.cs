using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Fyreplace.Events
{
    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<WeakReference<Delegate>>> eventHandlers = [];
        private readonly ConditionalWeakTable<object, IList<Delegate>> strongReferences = [];

        public void Subscribe<T>(object? target, Func<T, Task> handler) where T : IEvent
        {
            if (!eventHandlers.TryGetValue(typeof(T), out var handlers))
            {
                handlers = [];
                eventHandlers[typeof(T)] = handlers;
            }

            strongReferences.GetValue((target ?? handler.Target)!, _ => []).Add(handler);
            handlers.Add(new WeakReference<Delegate>(handler));
        }

        public void Unsubscribe<T>(Func<T, Task> handler) where T : IEvent
        {
            if (eventHandlers.TryGetValue(typeof(T), out var handlers))
            {
                handlers.RemoveAll(x => x.TryGetTarget(out var target) && target == handler as Delegate);
            }
        }

        public Task PublishAsync(IEvent someEvent)
        {
            if (!eventHandlers.TryGetValue(someEvent.GetType(), out var handlers))
            {
                return Task.CompletedTask;
            }

            var tasks = new List<Task>();
            var deadHandlers = new List<WeakReference<Delegate>>();

            foreach (var handler in handlers)
            {
                if (handler.TryGetTarget(out var target))
                {
                    tasks.Add((Task)target.DynamicInvoke(someEvent)!);
                }
                else
                {
                    deadHandlers.Add(handler);
                }
            }

            foreach (var handler in deadHandlers)
            {
                handlers.Remove(handler);
            }

            return Task.WhenAll(tasks);
        }
    }
}
