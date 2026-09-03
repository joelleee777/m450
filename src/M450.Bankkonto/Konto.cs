namespace M450.Bankkonto;

/// <summary>
/// Basisklasse für alle Kontoarten. Regelt Einzahlen, Beziehen, Transfer,
/// Zins und Abschluss. Wie viel überzogen werden darf, legt jede Unterklasse
/// selbst fest.
/// </summary>
public abstract class Konto
{
    private const int BankjahrInTagen = 360;
    private static long _letzteKontoNummer = 100_000;

    protected Konto(
        decimal aktivZins,
        decimal passivZins,
        decimal startGuthaben = 0m)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(aktivZins);
        ArgumentOutOfRangeException.ThrowIfNegative(passivZins);

        KontoNummer = Interlocked.Increment(ref _letzteKontoNummer);
        AktivZins = aktivZins;
        PassivZins = passivZins;
        Guthaben = startGuthaben;
    }

    public long KontoNummer { get; }

    public decimal Guthaben { get; private set; }

    public decimal AktivZins { get; }

    public decimal PassivZins { get; }

    public decimal AufgelaufenerZins { get; private set; }

    public void ZahleEin(decimal betrag)
    {
        StellePositivenBetragSicher(betrag);
        Guthaben += betrag;
    }

    public void Beziehe(decimal betrag)
    {
        StellePositivenBetragSicher(betrag);
        var neuesGuthaben = Guthaben - betrag;
        PruefeBezugErlaubt(betrag, neuesGuthaben);
        Guthaben = neuesGuthaben;
    }

    public void Transferiere(Konto konto, decimal betrag)
    {
        ArgumentNullException.ThrowIfNull(konto);
        StellePositivenBetragSicher(betrag);

        if (ReferenceEquals(this, konto))
        {
            throw new ArgumentException(
                "Quell- und Zielkonto müssen verschieden sein.",
                nameof(konto));
        }

        Beziehe(betrag);
        konto.ZahleEin(betrag);
    }

    public void SchreibeZinsGut(int anzTage)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(anzTage);

        var zinsSatz = Guthaben >= 0m ? AktivZins : PassivZins;
        AufgelaufenerZins += Guthaben * zinsSatz * anzTage
            / (100m * BankjahrInTagen);
    }

    public void SchliesseKontoAb()
    {
        Guthaben += AufgelaufenerZins;
        AufgelaufenerZins = 0m;
    }

    /// <summary>
    /// Prüft, ob ein Bezug erlaubt ist. <paramref name="neuesGuthaben"/> ist der
    /// Kontostand nach dem Bezug. Wenn die Regel verletzt wird, wirft die Methode
    /// eine Exception.
    /// </summary>
    protected abstract void PruefeBezugErlaubt(decimal betrag, decimal neuesGuthaben);

    private static void StellePositivenBetragSicher(decimal betrag)
    {
        if (betrag <= 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(betrag),
                "Der Betrag muss grösser als null sein.");
        }
    }
}
