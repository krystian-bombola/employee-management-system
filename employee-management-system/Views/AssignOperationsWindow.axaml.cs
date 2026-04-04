using Avalonia.Controls;
using employee_management_system.ViewModels;

namespace employee_management_system.Views;

public partial class AssignOperationsWindow : Window
{
    public AssignOperationsWindow() => InitializeComponent();

    public AssignOperationsWindow(AssignOperationsViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseAction = Close;
    }
}
