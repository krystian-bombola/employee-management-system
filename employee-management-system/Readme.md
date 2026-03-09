# Employee Management System

## Stan obecny
Aplikacja w Avalonia z bazą SQLite (`produkcja.db`).  

**ADMIN**:  
- Dodaje/usuwa pracowników, operacje i zlecenia.  
- Widzi podgląd czasu produkcji wszystkich pracowników.  

**Użytkownik**:  
- Loguje się poprzez `Identyfikator` i numer zlecenia.  
- Może rozpocząć i zakończyć operację.  
- Lista operacji jest obecnie statyczna (wszystkie operacje dostępne dla użytkownika).  

---

## Wymagania
- .NET 7 SDK  
- IDE: Visual Studio / Rider / VS Code  

---

## Uruchamianie
1. Sklonować repozytorium i przejść do folderu projektu.  
2. Otworzyć projekt w IDE lub uruchomić w terminalu:  
```bash
dotnet build
dotnet run --project employee_management_system

Baza SQLite zostanie utworzona automatycznie przy pierwszym uruchomieniu.

Logowanie

ADMIN: Identyfikator = admin (numer zlecenia dowolny jeszcze) 

Użytkownik: Identyfikator zgodny z Uzytkownicy.Identyfikator, numer zlecenia zgodny z Zlecenia.Id.
przykładowe na ten moment : Identyfikator = 123, numer zlecenia = 8888 (ale możecie na adminie dodawać jakie chcecie)
TODO / do poprawy

Poprawić logowanie – dodać dodatkowe zabezpieczenia (hashowanie haseł, sprawdzanie uprawnień).

Lista operacji dla użytkownika i admina powinna pobierać tylko operacje przypisane do aktualnego zlecenia (OperacjaPrzypisana).

Dodać walidację przy dodawaniu użytkowników, operacji i zleceń.