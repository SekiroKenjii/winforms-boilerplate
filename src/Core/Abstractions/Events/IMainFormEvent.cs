namespace Core.Abstractions.Events;

public interface IMainFormEvent : IFormEvent
{
    Action<bool>? OnShutdownApplication { get; set; }
}
