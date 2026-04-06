using CommunityToolkit.Mvvm.Input;

namespace employee_management_system.ViewModels;

public partial class MessageDialogViewModel
{
    private readonly System.Action _closeAction;

    public string WindowTitle { get; }
    public string Message { get; }

    public MessageDialogViewModel(string windowTitle, string message, System.Action closeAction)
    {
        WindowTitle = windowTitle;
        Message = message;
        _closeAction = closeAction;
    }

    [RelayCommand]
    private void Close() => _closeAction();
}
