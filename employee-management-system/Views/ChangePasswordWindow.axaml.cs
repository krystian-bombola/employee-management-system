using Avalonia.Controls;
using employee_management_system.ViewModels;

namespace employee_management_system.Views;

public partial class ChangePasswordWindow : Window
{
    public ChangePasswordWindow()
    {
        InitializeComponent();
    }

    public ChangePasswordWindow(ChangePasswordViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseAction = Close;
    }
}