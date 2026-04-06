using CommunityToolkit.Mvvm.Input;

namespace employee_management_system.ViewModels;

public partial class ConfirmActionViewModel
{
    private readonly System.Action<bool> _closeAction;

    public string WindowTitle { get; }
    public string Message { get; }
    public string ConfirmButtonText { get; }
    public string CancelButtonText { get; }

    public ConfirmActionViewModel(
        string windowTitle,
        string message,
        string confirmButtonText,
        string cancelButtonText,
        System.Action<bool> closeAction)
    {
        WindowTitle = windowTitle;
        Message = message;
        ConfirmButtonText = confirmButtonText;
        CancelButtonText = cancelButtonText;
        _closeAction = closeAction;
    }

    [RelayCommand]
    private void Confirm() => _closeAction(true);

    [RelayCommand]
    private void Cancel() => _closeAction(false);
}
