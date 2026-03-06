using System;

public class ZarzadzanieProdukcja
{
    public int Id { get; set; }

    public int IdPracownika { get; set; }
    public Uzytkownik Uzytkownicy { get; set; }

    public int IdOperacjiPrzypisanej { get; set; }
    public OperacjaPrzypisana OperacjaPrzypisana { get; set; }

    public TimeSpan StartPracy { get; set; }
    public TimeSpan KoniecPracy { get; set; }
}