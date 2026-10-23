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
        decimal startGuthaben = 0m,
        KontoStatus status = KontoStatus.Standard)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(aktivZins);
        ArgumentOutOfRangeException.ThrowIfNegative(passivZins);

        KontoNummer = Interlocked.Increment(ref _letzteKontoNummer);
        AktivZins = aktivZins;
        PassivZins = passivZins;
        Guthaben = startGuthaben;
        Status = status;
    }

    public long KontoNummer { get; }

    public decimal Guthaben { get; private set; }

    public decimal AktivZins { get; }

    public decimal PassivZins { get; }

    public KontoStatus Status { get; }

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

        var zinsSatz = Guthaben >= 0m ? ErmittleEffektivenAktivZins() : PassivZins;
        AufgelaufenerZins += Guthaben * zinsSatz * anzTage
            / (100m * BankjahrInTagen);
    }

    /// <summary>
    /// Ermittelt den effektiven Aktivzins anhand von Guthabenstufen und
    /// Status. Ab CHF 100'000 ist keine Geschäftsregel definiert.
    /// </summary>
    private decimal ErmittleEffektivenAktivZins()
    {
        if (Guthaben < 10_000m)
        {
            return AktivZins;
        }

        if (Guthaben < 50_000m)
        {
            return AktivZins + 0.5m;
        }

        if (Guthaben < 100_000m)
        {
            return Status == KontoStatus.VIP
                ? AktivZins + 1.5m
                : AktivZins + 0.75m;
        }

        throw new InvalidOperationException(
            "Für Guthaben ab CHF 100'000 ist kein Zinssatz definiert.");
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
