namespace M450.Bankkonto.Tests;

public sealed class JugendkontoTests
{
    [Fact]
    public void Beziehe_BisZumVorhandenenGuthaben_IstErlaubt()
    {
        // Arrange
        var konto = new Jugendkonto(aktivZins: 1.5m, passivZins: 5m, startGuthaben: 100m);

        // Act
        konto.Beziehe(100m);

        // Assert
        Assert.Equal(0m, konto.Guthaben);
    }

    [Fact]
    public void Beziehe_UeberVorhandenesGuthaben_WirdAbgelehnt()
    {
        // Arrange
        var konto = new Jugendkonto(aktivZins: 1.5m, passivZins: 5m, startGuthaben: 100m);

        // Act
        var exception = Record.Exception(() => konto.Beziehe(101m));

        // Assert
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal(100m, konto.Guthaben);
    }
}
