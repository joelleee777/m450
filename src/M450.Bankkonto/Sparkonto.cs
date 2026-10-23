namespace M450.Bankkonto;

/// <summary>
/// Sparkonto. Darf nicht ins Minus gehen.
/// </summary>
public sealed class Sparkonto : Konto
{
    public Sparkonto(
        decimal aktivZins,
        decimal passivZins,
        decimal startGuthaben = 0m,
        KontoStatus status = KontoStatus.Standard)
        : base(aktivZins, passivZins, startGuthaben, status)
    {
    }

    protected override void PruefeBezugErlaubt(decimal betrag, decimal neuesGuthaben)
    {
        if (neuesGuthaben < 0m)
        {
            throw new InvalidOperationException(
                "Ein Sparkonto kann nicht überzogen werden.");
        }
    }
}
