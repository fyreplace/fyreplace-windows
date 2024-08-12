namespace Fyreplace.Events
{
    public interface IEvent { }

    public readonly record struct FailureEvent(string Title = "Error_Unknown_Title", string Message = "Error_Unknown_Message") : IEvent { }
}
