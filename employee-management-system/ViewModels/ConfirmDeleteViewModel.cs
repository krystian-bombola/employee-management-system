using CommunityToolkit.Mvvm.Input;

namespace employee_management_system.ViewModels;

public partial class ConfirmDeleteViewModel
{
    private readonly System.Action<bool> _closeAction;

    public string WindowTitle { get; }
    public string Message { get; }

    public ConfirmDeleteViewModel(string windowTitle, string message, System.Action<bool> closeAction)
    {
        WindowTitle = windowTitle;
        Message = message;
        _closeAction = closeAction;
    }

    [RelayCommand]
    private void Confirm() => _closeAction(true);

    [RelayCommand]
    private void Cancel() => _closeAction(false);
}
