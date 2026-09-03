namespace M450.Bankkonto.Tests;

/// <summary>
/// Einfaches Konto ohne eigene Regeln, nur für Tests. Erlaubt jeden Bezug,
/// auch ins Minus. Kein echtes Produkt-Konto.
/// </summary>
internal sealed class TestKonto : Konto
{
    public TestKonto(
        decimal aktivZins,
        decimal passivZins,
        decimal startGuthaben = 0m)
        : base(aktivZins, passivZins, startGuthaben)
    {
    }

    protected override void PruefeBezugErlaubt(decimal betrag, decimal neuesGuthaben)
    {
    }
}
