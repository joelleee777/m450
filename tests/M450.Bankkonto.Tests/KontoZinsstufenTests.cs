namespace M450.Bankkonto.Tests;

/// <summary>
/// Äquivalenzklassen- und Grenzwerttests für die Guthabenstufen aus
/// Aufgabe 5-3. Grundlage: AktivZins 3.6%, PassivZins 7.2%. Bei 360 Tagen
/// entspricht der Zins genau Guthaben * Zinssatz / 100, was die erwarteten
/// Werte einfach nachrechenbar macht.
/// </summary>
public sealed class KontoZinsstufenTests
{
    // Spezialfall: negatives Guthaben -> weiterhin PassivZins (unverändert
    // seit Bankkonto II).
    [Fact]
    public void Grenzwert_KnappUnterNull_VerwendetPassivzins()
    {
        PruefeZinssatz(guthaben: -0.01m, erwarteterZinssatz: 7.2m);
    }

    [Fact]
    public void Grenzwert_Null_VerwendetAktivzins()
    {
        PruefeZinssatz(guthaben: 0.00m, erwarteterZinssatz: 3.6m);
    }

    [Fact]
    public void Grenzwert_KnappUeberNull_VerwendetAktivzins()
    {
        PruefeZinssatz(guthaben: 0.01m, erwarteterZinssatz: 3.6m);
    }

    // EK1: 0 <= g < 10'000, Status egal -> AktivZins
    [Fact]
    public void EK1_Standard_VerwendetAktivzins()
    {
        PruefeZinssatz(guthaben: 5_000m, status: KontoStatus.Standard, erwarteterZinssatz: 3.6m);
    }

    [Fact]
    public void EK1_VIP_VerwendetAuchAktivzins()
    {
        PruefeZinssatz(guthaben: 5_000m, status: KontoStatus.VIP, erwarteterZinssatz: 3.6m);
    }

    [Fact]
    public void Grenzwert_KnappUnter10000_VerwendetAktivzins()
    {
        PruefeZinssatz(guthaben: 9_999.99m, erwarteterZinssatz: 3.6m);
    }

    [Fact]
    public void Grenzwert_10000_VerwendetAktivzinsPlus05()
    {
        PruefeZinssatz(guthaben: 10_000.00m, erwarteterZinssatz: 4.1m);
    }

    [Fact]
    public void Grenzwert_KnappUeber10000_VerwendetAktivzinsPlus05()
    {
        PruefeZinssatz(guthaben: 10_000.01m, erwarteterZinssatz: 4.1m);
    }

    // EK2: 10'000 <= g < 50'000, Status egal -> AktivZins + 0.5
    [Fact]
    public void EK2_Standard_VerwendetAktivzinsPlus05()
    {
        PruefeZinssatz(guthaben: 25_000m, status: KontoStatus.Standard, erwarteterZinssatz: 4.1m);
    }

    [Fact]
    public void EK2_VIP_VerwendetAuchAktivzinsPlus05()
    {
        PruefeZinssatz(guthaben: 25_000m, status: KontoStatus.VIP, erwarteterZinssatz: 4.1m);
    }

    [Fact]
    public void Grenzwert_KnappUnter50000_Standard_VerwendetPlus05()
    {
        PruefeZinssatz(guthaben: 49_999.99m, status: KontoStatus.Standard, erwarteterZinssatz: 4.1m);
    }

    [Fact]
    public void Grenzwert_KnappUnter50000_VIP_VerwendetPlus05()
    {
        PruefeZinssatz(guthaben: 49_999.99m, status: KontoStatus.VIP, erwarteterZinssatz: 4.1m);
    }

    // EK3: 50'000 <= g < 100'000, Standard -> AktivZins + 0.75
    [Fact]
    public void Grenzwert_50000_Standard_VerwendetPlus075()
    {
        PruefeZinssatz(guthaben: 50_000.00m, status: KontoStatus.Standard, erwarteterZinssatz: 4.35m);
    }

    [Fact]
    public void Grenzwert_KnappUeber50000_Standard_VerwendetPlus075()
    {
        PruefeZinssatz(guthaben: 50_000.01m, status: KontoStatus.Standard, erwarteterZinssatz: 4.35m);
    }

    [Fact]
    public void EK3_Standard_VerwendetAktivzinsPlus075()
    {
        PruefeZinssatz(guthaben: 75_000m, status: KontoStatus.Standard, erwarteterZinssatz: 4.35m);
    }

    [Fact]
    public void Grenzwert_KnappUnter100000_Standard_VerwendetPlus075()
    {
        PruefeZinssatz(guthaben: 99_999.99m, status: KontoStatus.Standard, erwarteterZinssatz: 4.35m);
    }

    // EK4: 50'000 <= g < 100'000, VIP -> AktivZins + 1.5
    [Fact]
    public void Grenzwert_50000_VIP_VerwendetPlus15()
    {
        PruefeZinssatz(guthaben: 50_000.00m, status: KontoStatus.VIP, erwarteterZinssatz: 5.1m);
    }

    [Fact]
    public void Grenzwert_KnappUeber50000_VIP_VerwendetPlus15()
    {
        PruefeZinssatz(guthaben: 50_000.01m, status: KontoStatus.VIP, erwarteterZinssatz: 5.1m);
    }

    [Fact]
    public void EK4_VIP_VerwendetAktivzinsPlus15()
    {
        PruefeZinssatz(guthaben: 75_000m, status: KontoStatus.VIP, erwarteterZinssatz: 5.1m);
    }

    [Fact]
    public void Grenzwert_KnappUnter100000_VIP_VerwendetPlus15()
    {
        PruefeZinssatz(guthaben: 99_999.99m, status: KontoStatus.VIP, erwarteterZinssatz: 5.1m);
    }

    // Grenze 100'000: laut Aufgabenstellung kein Zinssatz definiert.
    [Fact]
    public void Grenzwert_100000_Standard_IstNichtDefiniert()
    {
        // Arrange
        var konto = NeuesKonto();
        konto.ZahleEin(100_000.00m);

        // Act
        var exception = Record.Exception(() => konto.SchreibeZinsGut(360));

        // Assert
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void Grenzwert_KnappUeber100000_VIP_IstNichtDefiniert()
    {
        // Arrange
        var konto = NeuesKonto(status: KontoStatus.VIP);
        konto.ZahleEin(100_000.01m);

        // Act
        var exception = Record.Exception(() => konto.SchreibeZinsGut(360));

        // Assert
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void Konstruktor_OhneStatus_IstStandard()
    {
        // Arrange & Act
        var konto = NeuesKonto();

        // Assert
        Assert.Equal(KontoStatus.Standard, konto.Status);
    }

    [Fact]
    public void Konstruktor_MitVIP_SetztStatus()
    {
        // Arrange & Act
        var konto = NeuesKonto(status: KontoStatus.VIP);

        // Assert
        Assert.Equal(KontoStatus.VIP, konto.Status);
    }

    private static void PruefeZinssatz(
        decimal guthaben,
        decimal erwarteterZinssatz,
        KontoStatus status = KontoStatus.Standard)
    {
        // Arrange
        var konto = NeuesKonto(status: status);

        if (guthaben > 0m)
        {
            konto.ZahleEin(guthaben);
        }
        else if (guthaben < 0m)
        {
            konto.Beziehe(-guthaben);
        }

        // Act
        konto.SchreibeZinsGut(360);

        // Assert
        Assert.Equal(guthaben * erwarteterZinssatz / 100m, konto.AufgelaufenerZins);
    }

    private static TestKonto NeuesKonto(
        decimal aktivZins = 3.6m,
        decimal passivZins = 7.2m,
        KontoStatus status = KontoStatus.Standard)
    {
        return new TestKonto(aktivZins, passivZins, status: status);
    }
}
