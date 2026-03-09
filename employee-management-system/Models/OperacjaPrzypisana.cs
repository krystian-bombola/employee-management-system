using System;
using System.Collections.Generic;

public class OperacjaPrzypisana
{
    public int Id { get; set; }

    public int IdZlecenia { get; set; }
    public Zlecenie Zlecenie { get; set; }

    public int IdOperacji { get; set; }
    public Operacja Operacja { get; set; }

    public int Kolejnosc { get; set; }
    public DateTime StartOperacji { get; set; }
    public DateTime KoniecOperacji { get; set; }
    public TimeSpan CzasWykonywania { get; set; }

    public ICollection<ZarzadzanieProdukcja> ZarzadzanieProdukcja { get; set; }
}