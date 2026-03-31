using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using employee_management_system.ViewModels;

namespace employee_management_system.Views;

public partial class AddJobWindow : Window
{
    public AddJobWindow()
    {
        InitializeComponent();
        
        // Tunelowanie klawiszy dla Enter/Esc
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
                if (DataContext is AddJobViewModel vm)
                {
                    vm.SaveCommand.Execute(null);
                    e.Handled = true;
                }
                return;
            }

            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                if (currentFocus == null)
                {
                    this.Find<TextBox>("JobNameTextBox")?.Focus();
                    e.Handled = true;
                    return;
                }

                var direction = e.Key == Key.Down ? NavigationDirection.Next : NavigationDirection.Previous;
                var next = KeyboardNavigationHandler.GetNext(currentFocus, direction);
                if (next is Control control)
                {
                    control.Focus();
                }
                e.Handled = true;
            }
        }, RoutingStrategies.Tunnel);

        Opened += (s, e) =>
        {
            this.Find<TextBox>("JobNameTextBox")?.Focus();
        };
    }

    public AddJobWindow(AddJobViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseAction = Close;
    }
}
