using System.Collections.Generic;

public class Uzytkownik
{
    public int Id { get; set; }
    public string Imie { get; set; }
    public string Nazwisko { get; set; }
    public string Identyfikator { get; set; }

    public ICollection<ZarzadzanieProdukcja> ZarzadzanieProdukcja { get; set; }
}