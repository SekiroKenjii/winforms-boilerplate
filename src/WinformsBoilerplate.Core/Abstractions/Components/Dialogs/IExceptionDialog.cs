namespace WinformsBoilerplate.Core.Abstractions.Components.Dialogs;

/// <summary>
/// Represents a dialog specifically designed for displaying exceptions.
/// The information in this dialog is <see cref="Entities.Systems.CallStack">CallStack</see> and typically used to inform the user about errors.
/// </summary>
public interface IExceptionDialog : IDialog;
