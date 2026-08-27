using M450.UnitTesting.App;

namespace M450.UnitTesting.App.Tests;

public class PriceCalculatorTests
{
    [Fact]
    public void CalculateTotal_WithoutDiscount_ReturnsSubtotal()
    {
        // Arrange
        var calculator = new PriceCalculator();
        var unitPrice = 25.50m;
        var quantity = 3;
        var expectedTotal = 76.50m;

        // Act
        var actualTotal = calculator.CalculateTotal(unitPrice, quantity);

        // Assert
        Assert.Equal(expectedTotal, actualTotal);
    }

    [Fact]
    public void CalculateTotal_WithDiscount_ReturnsDiscountedTotal()
    {
        // Arrange
        var calculator = new PriceCalculator();
        var unitPrice = 25.50m;
        var quantity = 3;
        var discountPercentage = 10m;
        var expectedTotal = 68.85m;

        // Act
        var actualTotal = calculator.CalculateTotal(unitPrice, quantity, discountPercentage);

        // Assert
        Assert.Equal(expectedTotal, actualTotal);
    }

    [Fact]
    public void CalculateTotal_WithQuantityOfZero_ReturnsZero()
    {
        // Arrange
        var calculator = new PriceCalculator();
        var unitPrice = 25.50m;
        var quantity = 0;
        var expectedTotal = 0m;

        // Act
        var actualTotal = calculator.CalculateTotal(unitPrice, quantity);

        // Assert
        Assert.Equal(expectedTotal, actualTotal);
    }

    [Fact]
    public void CalculateTotal_WithFullDiscount_ReturnsZero()
    {
        // Arrange
        var calculator = new PriceCalculator();
        var unitPrice = 25.50m;
        var quantity = 3;
        var discountPercentage = 100m;
        var expectedTotal = 0m;

        // Act
        var actualTotal = calculator.CalculateTotal(unitPrice, quantity, discountPercentage);

        // Assert
        Assert.Equal(expectedTotal, actualTotal);
    }

    [Fact]
    public void CalculateTotal_WithNegativeUnitPrice_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var calculator = new PriceCalculator();
        var unitPrice = -1m;
        var quantity = 3;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => calculator.CalculateTotal(unitPrice, quantity));
    }

    [Fact]
    public void CalculateTotal_WithNegativeQuantity_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var calculator = new PriceCalculator();
        var unitPrice = 25.50m;
        var quantity = -3;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => calculator.CalculateTotal(unitPrice, quantity));
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(100.01)]
    public void CalculateTotal_WithDiscountOutsideValidRange_ThrowsArgumentOutOfRangeException(
        double discountPercentage)
    {
        // Arrange
        var calculator = new PriceCalculator();
        var unitPrice = 25.50m;
        var quantity = 3;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => calculator.CalculateTotal(unitPrice, quantity, (decimal)discountPercentage));
    }
}
