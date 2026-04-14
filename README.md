# Dokumentacja Projektu: JKM

**Autorzy:**
1. Jakub Gutowski
2. Krystian Bombola
3. Jakub Gutowski

## 1. Informacje ogólne
Aplikacja **JKM** służy do zarządzania pracownikami, stanowiskami oraz zadaniami produkcyjnymi w przedsiębiorstwie. System oferuje panel administratora do zarządzania zasobami oraz panel użytkownika do logowania czasu pracy i postępów zadań.

---

## 2. Realizacja Kryteriów Oceny

### A. Wymagania minimalne (MVP)
*   **CRUD na co najmniej jednej encji**: System umożliwia pełne zarządzanie (dodawanie, wyświetlanie, edycję i usuwanie) dla wielu encji: `User`, `Position`, `Job` (Zlecenie) oraz `Operation`.
    *   *Implementacja*: Pliki w folderach `Repositories` (np. `UserRepository.cs`) oraz `ViewModels` (np. `AdminUsersSectionViewModel.cs`).
*   **Trwałość danych**: Zastosowano bazę danych **SQLite** wraz z **Entity Framework Core**, co gwarantuje pełną trwałość danych pomiędzy uruchomieniami aplikacji.
    *   *Implementacja*: `DatabaseContext.cs`, `DatabaseInitializer.cs`.
*   **Podstawowa logika biznesowa**: 
    1.  **Unikalność pola**: Weryfikacja unikalności loginu użytkownika przy tworzeniu konta.
    2.  **Walidacja haseł**: Mechanizm zmiany hasła z wymogiem poprawności starego hasła.
    3.  **Priorytety zadań**: Ograniczony zakres wartości dla priorytetów zleceń.
*   **Obsługa błędów i walidacja danych**: Implementacja `DialogService` oraz okien dialogowych (np. `MessageDialogView`) informujących o błędach (np. niepoprawne dane logowania, błędy bazy danych).
*   **UI zbudowane w XAML**: Interfejs stworzony w języku XAML (Avalonia UI) z wykorzystaniem layoutów `Grid`, `StackPanel`, `Border`.
*   **Data Binding i Commands**:
    *   Wykorzystano **Compiled Bindings** dla zwiększenia wydajności.
    *   **Kolekcje**: Wiązanie `ObservableCollection` do kontrolek typu `DataGrid` i `ListBox`.
    *   **Formularze**: Dwukierunkowe wiązanie do obiektów (np. w `EditUserViewModel`).
    *   **RelayCommand**: Akcje przycisków oparte na poleceniach z `CommunityToolkit.Mvvm`.

### B. Rozwinięcia i jakość
*   **Rozbudowa domeny i funkcjonalności**:
    *   **Wiele encji i relacje**: System zarządza relacjami między Użytkownikami, Stanowiskami a Zleceniami (np. przypisanie operacji do zlecenia).
    *   **Filtrowanie i wyszukiwanie**: Implementacja wyszukiwania w panelu administratora (np. wyszukiwanie użytkowników).
*   **Zaawansowane mechanizmy UI**:
    *   **Style i zasoby**: Definicje stylów w osobnych plikach (`Assets/Styles/Controls.axaml`, `Foundation.axaml`), zapewniające spójny wygląd (Theming).
    *   **Data Templates**: Customowe szablony dla elementów list i widoków szczegółowych.
*   **Architektura i dobre praktyki**:
    *   **MVVM**: Pełna separacja logiki od widoku (foldery `Views`, `ViewModels`, `Models`).
    *   **SQLite**: Profesjonalny system zapisu danych zamiast plików tekstowych.
    *   **Asynchroniczność**: Wykorzystanie słów kluczowych `async/await` przy operacjach bazodanowych i dialogach, co zapobiega "zamrażaniu" interfejsu.
    *   **Dependency Injection (uproszczone)**: Wykorzystanie serwisów (folder `Services`) do separacji logiki biznesowej od ViewModeli.

---

## 3. Struktura Projektu
- **Models**: Definicje danych (Użytkownik, Zlecenie, Stanowisko).
- **Repositories**: Warstwa dostępu do danych (EF Core).
- **Services**: Logika aplikacyjna (autoryzacja, zarządzanie dialogami).
- **ViewModels**: Logika prezentacji, obsługa komend i powiadomień o zmianach własności.
- **Views**: Definicje interfejsu użytkownika w XAML.

---

## 4. Instrukcja Uruchomienia
1. Wymagane środowisko: **.NET 10.0 SDK**.
2. Otwórz projekt w Visual Studio 2026 lub JetBrains Rider.
3. Przy pierwszym uruchomieniu `DatabaseInitializer` automatycznie utworzy bazę `database.db` i wypełni ją danymi początkowymi (o ile nie istnieją).
4. Domyślne dane logowania administratora (zależnie od inicjalizacji): `admin` / `admin123`.
