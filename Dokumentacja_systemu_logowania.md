# Dokumentacja systemu logowania

## 1. Cel systemu
System logowania w aplikacji `employee-management-system`:
- uwierzytelnia użytkownika po polu `Identyfikator`,
- wymaga podania `ID zlecenia`,
- przekierowuje użytkownika do odpowiedniego widoku zależnie od roli (`admin` / `user`).

## 2. Główne elementy

### UI logowania
Plik: `Views/LoginView.axaml`
- Pole `ID pracownika` -> binding do `Identyfikator`.
- Pole `ID zlecenia` -> binding do `OrderId`.
- Przycisk `Zaloguj` -> `LoginCommand`.
- Komunikat błędu -> `ErrorMessage` + `IsErrorVisible`.

### Logika logowania
Plik: `ViewModels/LoginViewModel.cs`
- Waliduje `Identyfikator` (nie może być pusty).
- Waliduje `OrderId` (nie może być puste).
- Pobiera użytkownika przez `DatabaseService.FindByIdentyfikator(...)`.
- Przekierowanie:
  - `admin` -> `AdminWindowViewModel`.
  - `user` -> `UserPanelViewModel(mainVm, employeeName, orderId)`.
- W razie braku użytkownika lub uprawnień ustawia komunikat błędu.

### Dostęp do danych użytkownika
Plik: `Data/DatabaseService.cs`
- Zapytanie SQL:
  - `SELECT Id, Imie, Nazwisko, Identyfikator FROM Uzytkownik WHERE Identyfikator = @id`
- Parametryzacja `@id` zabezpiecza przed SQL Injection.

## 3. Inicjalizacja i seed bazy

### Inicjalizator
Plik: `Data/DatabaseInitializer.cs`
- Tworzy tabele `Uzytkownik` i `Zlecenie` jeśli nie istnieją.
- Zapewnia obecność rekordów seed:
  - Użytkownik `admin` (Jan Kowalski),
  - Użytkownik `user` (Anna Nowak),
  - Zlecenie `aaaa`.
- Seed działa warunkowo: rekordy są dodawane tylko, jeśli ich brakuje.

### Start aplikacji
Plik: `Program.cs`
- Buduje ścieżkę bazy:
  - `Path.Combine(AppContext.BaseDirectory, "produkcja.db")`
- Wywołuje:
  - `DatabaseInitializer.Initialize(dbPath)`
  - `db.Database.EnsureCreated()`

## 4. Spójność ścieżki do bazy danych

Aby uniknąć problemów z różnymi katalogami roboczymi, ścieżka do `produkcja.db` jest oparta o `AppContext.BaseDirectory` w:
- `Program.cs`
- `LoginViewModel.cs`
- `DatabaseContext.cs`

To zapewnia, że logowanie i operacje EF Core pracują na tym samym pliku bazy.

## 5. Dane testowe do logowania

Po starcie aplikacji (po inicjalizacji/seedingu) dostępne są konta:
- `admin`
- `user`

Wymagane jest również podanie `ID zlecenia` (np. `1` lub inna wartość testowa używana w przepływie UI).

## 6. Typowe komunikaty błędów

- `Wpisz identyfikator.` -> brak `Identyfikator`.
- `Wpisz ID zlecenia.` -> brak `OrderId`.
- `Nie znaleziono użytkownika.` -> brak rekordu w `Uzytkownik` dla podanego identyfikatora.
- `Brak uprawnień dla tego konta.` -> identyfikator inny niż `admin` / `user`.

## 7. Przepływ logowania (skrót)

1. Użytkownik wpisuje `Identyfikator` i `ID zlecenia`.
2. `LoginCommand` uruchamia walidację pól.
3. `DatabaseService` wyszukuje użytkownika po `Identyfikator`.
4. Jeśli znaleziony:
   - `admin` -> panel administratora,
   - `user` -> panel użytkownika.
5. Jeśli nie znaleziony -> komunikat błędu.
