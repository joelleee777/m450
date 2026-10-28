
using Xunit;
using TestDoublesExercise;

namespace TestDoublesExercise.Tests;

public class ReservationServiceTests
{
    [Fact]
    public void Reserve_GenugPlaetze_ReturnsTrue()
    {
        // Arrange
        var availability = new SeatAvailabilityStub(3);
        var repository = new ReservationRepositoryStub();
        var confirmation = new ConfirmationSenderStub();

        var service = new ReservationService(
            availability,
            repository,
            confirmation
        );

        // Act
        bool result = service.Reserve("test@example.com", "EVENT1", 2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Reserve_ZuWenigPlaetze_ReturnsFalse()
    {
        // Arrange
        var availability = new SeatAvailabilityStub(1);
        var repository = new ReservationRepositoryStub();
        var confirmation = new ConfirmationSenderStub();

        var service = new ReservationService(
            availability,
            repository,
            confirmation
        );

        // Act
        bool result = service.Reserve("test@example.com", "EVENT1", 2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Reserve_GenauGenugPlaetze_ReturnsTrue()
    {
        // Arrange
        var availability = new SeatAvailabilityStub(2);
        var repository = new ReservationRepositoryStub();
        var confirmation = new ConfirmationSenderStub();

        var service = new ReservationService(
            availability,
            repository,
            confirmation
        );

        // Act
        bool result = service.Reserve("test@example.com", "EVENT1", 2);

        // Assert
        Assert.True(result);
    }
}
