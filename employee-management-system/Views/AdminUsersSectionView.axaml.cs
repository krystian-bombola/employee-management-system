using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using employee_management_system.ViewModels;

namespace employee_management_system.Views;

public partial class AdminUsersSectionView : UserControl
{
    public AdminUsersSectionView()
    {
        InitializeComponent();
        
        // Używamy Tunelu, aby przechwycić klawisze zanim przyciski je obsłużą
        this.AddHandler(KeyDownEvent, (s, e) =>
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var focusManager = topLevel?.FocusManager;
            var currentFocus = focusManager?.GetFocusedElement() as Control;

            if (DataContext is AdminUsersSectionViewModel vm && vm.IsDeleteConfirmationVisible)
            {
                if (e.Key == Avalonia.Input.Key.Escape)
                {
                    vm.CancelDeleteCommand.Execute(null);
                    e.Handled = true;
                }
                else if (e.Key == Avalonia.Input.Key.Enter)
                {
                    vm.ConfirmDeleteCommand.Execute(null);
                    e.Handled = true;
                }
                return;
            }

            // Obsługa skoku do listy przy strzałkach (jeśli nie jesteśmy jeszcze w liście)
            if (e.Key == Avalonia.Input.Key.Down || e.Key == Avalonia.Input.Key.Up)
            {
                var listBox = this.Find<ListBox>("UsersListBox");
                bool isFocusInList = listBox != null && currentFocus != null && listBox.IsVisualAncestorOf(currentFocus);

                if (!isFocusInList)
                {
                    if (listBox != null && listBox.ItemCount > 0)
                    {
                        if (listBox.SelectedItem == null)
                        {
                            listBox.SelectedIndex = 0;
                        }
                        
                        listBox.Focus();
                        e.Handled = true; // Przejmujemy pierwsze naciśnięcie, aby przenieść fokus i zaznaczenie
                    }
                }
            }
        }, RoutingStrategies.Tunnel);

        DataContextChanged += (s, e) =>
        {
            if (DataContext is AdminUsersSectionViewModel vm)
            {
                vm.PropertyChanged += (ps, pe) =>
                {
                    if (pe.PropertyName == nameof(AdminUsersSectionViewModel.IsDeleteConfirmationVisible))
                    {
                        if (vm.IsDeleteConfirmationVisible)
                        {
                            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                            {
                                var button = this.Find<Button>("CancelDeleteButton");
                                button?.Focus();
                            }, Avalonia.Threading.DispatcherPriority.Input);
                        }
                        else
                        {
                            // Powrót fokusu na listę po zamknięciu okna potwierdzenia
                            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                            {
                                this.Find<ListBox>("UsersListBox")?.Focus();
                            }, Avalonia.Threading.DispatcherPriority.Input);
                        }
                    }
                    else if (pe.PropertyName == nameof(AdminUsersSectionViewModel.SelectedUser) && vm.SelectedUser != null)
                    {
                        // Upewnij się, że element jest widoczny i sfokusowany po zmianie (np. po edycji)
                        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                        {
                            var listBox = this.Find<ListBox>("UsersListBox");
                            if (listBox != null && listBox.SelectedItem != null)
                            {
                                listBox.Focus();
                            }
                        }, Avalonia.Threading.DispatcherPriority.Input);
                    }
                };
            }
        };
    }
}
