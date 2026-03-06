using System.Collections.Generic;

public class Operacja
{
    public int Id { get; set; }
    public string NazwaOperacji { get; set; }

    public ICollection<OperacjaPrzypisana> OperacjePrzypisane { get; set; }
}