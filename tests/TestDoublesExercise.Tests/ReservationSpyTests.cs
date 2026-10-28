
using Xunit;
using TestDoublesExercise;

namespace TestDoublesExercise.Tests;

public class ReservationSpyTests
{
    [Fact]
    public void Reserve_Erfolgreich_SendetEineBestaetigung()
    {
        // Arrange
        var availability = new SeatAvailabilityStub(3);
        var repository = new ReservationRepositoryStub(true);
        var spy = new ConfirmationSpy();

        var service = new ReservationService(
            availability,
            repository,
            spy
        );

        // Act
        bool result = service.Reserve("test@example.com", "EVENT1", 2);

        // Assert
        Assert.True(result);
        Assert.Single(spy.SentReservations);

        var reservation = spy.SentReservations[0];

        Assert.Equal("test@example.com", reservation.CustomerEmail);
        Assert.Equal("EVENT1", reservation.EventCode);
        Assert.Equal(2, reservation.SeatCount);
    }

    [Fact]
    public void Reserve_ZuWenigPlaetze_SendetKeineBestaetigung()
    {
        // Arrange
        var availability = new SeatAvailabilityStub(1);
        var repository = new ReservationRepositoryStub(true);
        var spy = new ConfirmationSpy();

        var service = new ReservationService(
            availability,
            repository,
            spy
        );

        // Act
        bool result = service.Reserve("test@example.com", "EVENT1", 2);

        // Assert
        Assert.False(result);
        Assert.Empty(spy.SentReservations);
    }

    [Fact]
    public void Reserve_SpeichernFehlgeschlagen_SendetKeineBestaetigung()
    {
        // Arrange
        var availability = new SeatAvailabilityStub(3);
        var repository = new ReservationRepositoryStub(false);
        var spy = new ConfirmationSpy();

        var service = new ReservationService(
            availability,
            repository,
            spy
        );

        // Act
        bool result = service.Reserve("test@example.com", "EVENT1", 2);

        // Assert
        Assert.False(result);
        Assert.Empty(spy.SentReservations);
    }
}
