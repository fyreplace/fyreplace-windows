using System;

namespace Fyreplace.Events
{
    public interface IEvent { }

    public record class FailureEvent(string Key = "Error_Unknown") : IEvent
    {
        public string Title => $"{Key}/{nameof(Title)}";
        public string Message => $"{Key}/{nameof(Message)}";
    }

    public record class PreferenceChangedEvent(string Name) : IEvent { }

    public record class SecretChangedEvent(string Name) : IEvent { }

    public record class ModelChangedEvent(Guid Id, string PropertyName) : IEvent { }
}
