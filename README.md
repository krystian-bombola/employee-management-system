# Dokumentacja zmian – Employee Management System

## Zakres wykonanych zmian

Poniżej znajduje się podsumowanie zmian wdrożonych w aplikacji w ostatnich iteracjach prac.

## 1. Panel użytkownika – wybór zlecenia i operacji

- Dodano pole wyboru **zlecenia** nad polem wyboru operacji.
- Pole zlecenia i operacji zostały podłączone do danych z bazy.

### Format wyświetlania pozycji

- **Zlecenia**: `Id - Nazwa zlecenia - Opis zlecenia`
- **Operacje**: `Nazwa operacji - Opis operacji`

Źródła danych:
- Zlecenia: `Jobs.Description`
- Operacje: `Operations.Description`

## 2. Panel użytkownika – sekcja „Praca”

- Zmieniono nagłówek z „Wykonywana operacja” na **„Praca”**.
- W trakcie pracy wyświetlany jest format:
  - `Nazwa zlecenia - Nazwa operacji`

## 3. Ograniczenie widocznych zleceń dla użytkownika

Dla użytkownika widoczne są zlecenia o statusach:
- `Nowe`
- `W produkcji`
- `W trakcie`

## 4. Automatyczna zmiana statusu zlecenia

Przy pierwszym uruchomieniu pracy (`Rozpocznij`) na zleceniu o statusie `Nowe`:
- status automatycznie zmienia się na `W produkcji`.

Implementacja została wykonana z użyciem **Entity Framework** podczas obsługi startu pracy.

## 5. Panel admina – opis zlecenia

Dodano pełną obsługę opisu zlecenia:

- W modelu `Job` dodano pole `Description`.
- W bazie danych (`Jobs`) dodano kolumnę `Description` oraz mechanizm migracji kolumny dla istniejącej bazy (`EnsureColumnExists`).
- W panelu admina (zakładka zlecenia) po lewej stronie dodano pole formularza:
  - **„Opis zlecenia”**
- W liście zleceń dodano kolumnę:
  - **„Opis zlecenia”**
- Wyszukiwanie zleceń uwzględnia również opis.

## 6. Zabezpieczenia pracy użytkownika

Po uruchomieniu pracy:
- listy wyboru zlecenia i operacji są blokowane (nie można ich rozwinąć).

Odblokowanie następuje po:
- `Wstrzymaj`
- `Zakończ`

## 7. Potwierdzenie przy „Zakończ”

Dodano okno potwierdzenia po kliknięciu `Zakończ` z treścią w stylu:
- „Czy na pewno operacja została zrealizowana?”

Dostępne akcje:
- `Nie` – anulowanie zakończenia
- `Tak, zakończ` – zakończenie pracy

## 8. Uwagi techniczne

- Zmiany objęły warstwę **View**, **ViewModel**, modele domenowe oraz inicjalizację bazy.
- Komendy i logika UI zostały dostosowane do nowych stanów (blokowanie list, potwierdzenie zakończenia, zmiany statusów).

## 9. Znany problem środowiskowy

W środowisku deweloperskim występuje niezależny problem przy buildzie:

