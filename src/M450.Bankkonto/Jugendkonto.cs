namespace M450.Bankkonto;

/// <summary>
/// Jugendkonto. Darf nicht ins Minus gehen. Altersprüfung und Bezugslimite
/// fehlen noch.
/// </summary>
public sealed class Jugendkonto : Konto
{
    public Jugendkonto(
        decimal aktivZins,
        decimal passivZins,
        decimal startGuthaben = 0m)
        : base(aktivZins, passivZins, startGuthaben)
    {
    }

    protected override void PruefeBezugErlaubt(decimal betrag, decimal neuesGuthaben)
    {
        if (neuesGuthaben < 0m)
        {
            throw new InvalidOperationException(
                "Ein Jugendkonto kann nicht überzogen werden.");
        }
    }
}
