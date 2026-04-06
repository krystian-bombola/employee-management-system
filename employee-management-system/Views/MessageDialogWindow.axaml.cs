using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using employee_management_system.ViewModels;

namespace employee_management_system.Views;

public partial class MessageDialogWindow : Window
{
    public MessageDialogWindow()
    {
        InitializeComponent();

        this.AddHandler(KeyDownEvent, (s, e) =>
        {
            if (DataContext is not MessageDialogViewModel vm)
                return;

            if (e.Key == Key.Escape || e.Key == Key.Enter)
            {
                vm.CloseCommand.Execute(null);
                e.Handled = true;
            }
        }, RoutingStrategies.Tunnel);

        Opened += (s, e) => this.Find<Button>("CloseButton")?.Focus();
    }
}
