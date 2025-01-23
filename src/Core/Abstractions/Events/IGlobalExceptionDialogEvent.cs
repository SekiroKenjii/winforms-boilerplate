using Core.Structs;

namespace Core.Abstractions.Events;

public interface IGlobalExceptionDialogEvent : IFormEvent
{
    Action<Dictionary<string, IEnumerable<CallStack>>>? OnShowGlobalExceptionDialog { get; set; }
}
