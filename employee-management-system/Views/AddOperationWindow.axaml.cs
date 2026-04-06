using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using employee_management_system.ViewModels;

namespace employee_management_system.Views;

public partial class AddOperationWindow : Window
{
    public AddOperationWindow()
    {
        InitializeComponent();

        this.AddHandler(KeyDownEvent, (s, e) =>
        {
            var focusManager = TopLevel.GetTopLevel(this)?.FocusManager;
            var currentFocus = focusManager?.GetFocusedElement() as Control;

            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter)
            {
                if (currentFocus is TextBox textBox && textBox.AcceptsReturn)
                {
                    return;
                }

                if (DataContext is AddOperationViewModel vm)
                {
                    vm.SaveCommand.Execute(null);
                    e.Handled = true;
                }
                return;
            }
        }, RoutingStrategies.Tunnel);

        Opened += (s, e) =>
        {
            var operationNameTextBox = this.Find<TextBox>("OperationNameTextBox");
            operationNameTextBox?.Focus();
        };
    }

    public AddOperationWindow(AddOperationViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseAction = Close;
    }
}
