namespace Fyreplace.Events
{
    public interface IEvent { }

    public readonly record struct FailureEvent(string Key = "Error_Unknown") : IEvent
    {
        public string Title => $"{Key}/{nameof(Title)}";
        public string Message => $"{Key}/{nameof(Message)}";
    }

    public readonly record struct PreferenceChangedEvent(string Name) : IEvent { }

    public readonly record struct SecretChangedEvent(string Name) : IEvent { }
}
