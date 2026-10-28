
using Xunit;
using TestDoublesExercise;

namespace TestDoublesExercise.Tests;

public class ReservationValidationTests
{
    private class AvailabilityStub : ISeatAvailability
    {
        public int CallCount { get; private set; }

        public int GetAvailableSeats(string eventCode)
        {
            CallCount++;
            return 10;
        }
    }

    private class RepositoryStub : IReservationRepository
    {
        public int CallCount { get; private set; }

        public bool TrySave(Reservation reservation)
        {
            CallCount++;
            return true;
        }
    }

    private class ConfirmationStub : IConfirmationSender
    {
        public int CallCount { get; private set; }

        public void Send(Reservation reservation)
        {
            CallCount++;
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Reserve_UngueltigePlatzanzahl_WirftException(int seatCount)
    {
        // Arrange
        var availability = new AvailabilityStub();
        var repository = new RepositoryStub();
        var confirmation = new ConfirmationStub();

        var service = new ReservationService(
            availability,
            repository,
            confirmation
        );

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.Reserve("test@example.com", "EVENT1", seatCount)
        );

        Assert.Equal(0, availability.CallCount);
        Assert.Equal(0, repository.CallCount);
        Assert.Equal(0, confirmation.CallCount);
    }

    [Fact]
    public void Reserve_LeererEventCode_WirftException()
    {
        // Arrange
        var availability = new AvailabilityStub();
        var repository = new RepositoryStub();
        var confirmation = new ConfirmationStub();

        var service = new ReservationService(
            availability,
            repository,
            confirmation
        );

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            service.Reserve("test@example.com", "", 2)
        );

        Assert.Equal(0, availability.CallCount);
        Assert.Equal(0, repository.CallCount);
        Assert.Equal(0, confirmation.CallCount);
    }
}
