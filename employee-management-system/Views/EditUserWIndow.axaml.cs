using Avalonia.Controls;
using employee_management_system.ViewModels;

namespace employee_management_system.Views;

public partial class EditUserWindow : Window
{
    public EditUserWindow()
    {
        InitializeComponent();
    }

    public EditUserWindow(EditUserViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseAction = Close;
    }
}