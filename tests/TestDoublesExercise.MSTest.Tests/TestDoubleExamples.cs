using Moq;

namespace TestDoublesExercise.MSTest.Tests;

// Unabhängiges Beispiel: Stub, Spy und Moq an einem kleinen Lagerbestand.
// Für ReservationService schreibt ihr eure eigenen Doubles.

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

[TestClass]
public sealed class TestDoubleExamples
{
    [TestMethod]
    public void Stub_LiefertFestenLagerbestand()
    {
        // Arrange
        var stock = new StockLevelStub("ART-1", stock: 2);
        var log = new StockMovementLogSpy();
        var warehouse = new WarehouseService(stock, log);

        // Act
        var result = warehouse.TryTakeOut("ART-1", 3);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(0, log.Calls.Count);
    }

    [TestMethod]
    public void Spy_ZeichnetDenAufrufAuf()
    {
        // Arrange
        var log = new StockMovementLogSpy();
        var warehouse = new WarehouseService(new StockLevelStub("ART-1", 5), log);

        // Act
        var result = warehouse.TryTakeOut("ART-1", 3);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(1, log.Calls.Count);
        Assert.AreEqual(("ART-1", -3), log.Calls[0]);
    }

    [TestMethod]
    public void Moq_LegtRueckgabewertFestUndPrueftDenAufruf()
    {
        // Arrange
        var stockMock = new Mock<IStockLevel>();
        stockMock.Setup(s => s.GetStock("ART-1")).Returns(5);
        var logMock = new Mock<IStockMovementLog>();
        var warehouse = new WarehouseService(stockMock.Object, logMock.Object);

        // Act
        var result = warehouse.TryTakeOut("ART-1", 3);

        // Assert
        Assert.IsTrue(result);
        logMock.Verify(l => l.Record("ART-1", -3), Times.Once());
    }

    private sealed class StockLevelStub(string articleCode, int stock) : IStockLevel
    {
        public int GetStock(string code) => code == articleCode ? stock : 0;
    }

    private sealed class StockMovementLogSpy : IStockMovementLog
    {
        public List<(string ArticleCode, int Delta)> Calls { get; } = [];

        public void Record(string articleCode, int delta) => Calls.Add((articleCode, delta));
    }
}
