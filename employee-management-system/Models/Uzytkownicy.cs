using System.Collections.Generic;

public class Uzytkownik
{
    public int Id { get; set; }
    public string Imie { get; set; } = string.Empty;
    public string Nazwisko { get; set; } = string.Empty;
    public string Identyfikator { get; set; } = string.Empty;

    public ICollection<ZarzadzanieProdukcja> ZarzadzanieProdukcja { get; set; } = new List<ZarzadzanieProdukcja>();
}