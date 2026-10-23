using Moq;

namespace TestDoublesExercise.Tests;

// Dieses Lagerbestandsbeispiel ist unabhängig von der Reservierungsaufgabe.
// Es zeigt lauffähig, wie ein selbst geschriebener Stub, ein selbst
// geschriebener Spy und Moq eingesetzt werden. Übertragt die Ideen auf
// ReservationService in ReservationServiceTests.cs.

public interface IStockLevel
{
    int GetStock(string articleCode);
}

public interface IStockMovementLog
{
    void Record(string articleCode, int delta);
}

public sealed class WarehouseService(IStockLevel stock, IStockMovementLog log)
{
    public bool TryTakeOut(string articleCode, int amount)
    {
        if (stock.GetStock(articleCode) < amount)
        {
            return false;
        }

        log.Record(articleCode, -amount);
        return true;
    }
}

public sealed class TestDoubleExamplesTests
{
    // Handgeschriebener Stub: liefert für einen festgelegten Artikel einen
    // festen Lagerbestand. Ein Stub liefert Daten für den Test, mehr nicht.
    private sealed class StockLevelStub(string articleCode, int stock) : IStockLevel
    {
        public int GetStock(string code) => code == articleCode ? stock : 0;
    }

    // Handgeschriebener Spy: zeichnet jeden Aufruf mitsamt Argumenten auf,
    // damit der Test danach prüfen kann, was passiert ist.
    private sealed class StockMovementLogSpy : IStockMovementLog
    {
        public List<(string ArticleCode, int Delta)> Calls { get; } = [];

        public void Record(string articleCode, int delta) => Calls.Add((articleCode, delta));
    }

    [Fact]
    public void TryTakeOut_GenugLagerbestand_GibtTrueZurueckUndProtokolliertBewegung()
    {
        // Arrange
        var stock = new StockLevelStub(articleCode: "ART-1", stock: 5);
        var log = new StockMovementLogSpy();
        var warehouse = new WarehouseService(stock, log);

        // Act
        var result = warehouse.TryTakeOut("ART-1", 3);

        // Assert
        Assert.True(result);
        var call = Assert.Single(log.Calls);
        Assert.Equal(("ART-1", -3), call);
    }

    [Fact]
    public void TryTakeOut_ZuWenigLagerbestand_GibtFalseZurueckUndProtokolliertNichts()
    {
        // Arrange
        var stock = new StockLevelStub(articleCode: "ART-1", stock: 2);
        var log = new StockMovementLogSpy();
        var warehouse = new WarehouseService(stock, log);

        // Act
        var result = warehouse.TryTakeOut("ART-1", 3);

        // Assert
        Assert.False(result);
        Assert.Empty(log.Calls);
    }

    [Fact]
    public void TryTakeOut_MitMoq_GibtTrueZurueckUndRuftRecordGenauEinmalMitErwartetenArgumentenAuf()
    {
        // Arrange
        var stockMock = new Mock<IStockLevel>();
        stockMock.Setup(s => s.GetStock("ART-1")).Returns(5);
        var logMock = new Mock<IStockMovementLog>();
        var warehouse = new WarehouseService(stockMock.Object, logMock.Object);

        // Act
        var result = warehouse.TryTakeOut("ART-1", 3);

        // Assert
        Assert.True(result);
        logMock.Verify(l => l.Record("ART-1", -3), Times.Once());
    }
}
