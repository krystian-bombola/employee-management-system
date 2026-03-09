using System;
using System.Collections.Generic;

public class Zlecenie
{
    public int Id { get; set; }
    public string NazwaZlecenia { get; set; }
    public DateTime DataUtworzenia { get; set; }
    public string Status { get; set; }

    public ICollection<OperacjaPrzypisana> OperacjePrzypisane { get; set; }
}