using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using employee_management_system.ViewModels;
using employee_management_system.Views;

namespace employee_management_system.Services;

public static class DialogService
{
    public static async Task<bool> ShowDeleteConfirmationAsync(string message, string title = "Potwierdzenie usunięcia")
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return false;

        var owner = desktop.Windows.LastOrDefault(w => w.IsActive) ?? desktop.MainWindow;
        if (owner is null)
            return false;

        var confirmed = false;
        ConfirmDeleteViewModel? vm = null;
        var window = new ConfirmDeleteWindow();
        vm = new ConfirmDeleteViewModel(title, message, result =>
        {
            confirmed = result;
            window.Close();
        });

        window.DataContext = vm;
        await window.ShowDialog(owner);
        return confirmed;
    }

    public static async Task ShowMessageAsync(string message, string title = "Informacja")
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;

        var owner = desktop.Windows.LastOrDefault(w => w.IsActive) ?? desktop.MainWindow;
        if (owner is null)
            return;

        var window = new MessageDialogWindow();
        var vm = new MessageDialogViewModel(title, message, window.Close);
        window.DataContext = vm;
        await window.ShowDialog(owner);
    }
}
