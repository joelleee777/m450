
using Xunit;
using Moq;
using TestDoublesExercise;

namespace TestDoublesExercise.Tests;

public class ReservationMoqTests
{
    [Fact]
    public void Reserve_GenugPlaetze_SpeichertReservierung()
    {
        // Arrange
        var availability = new SeatAvailabilityStub(3);
        var repositoryMock = new Mock<IReservationRepository>();
        var spy = new ConfirmationSpy();

        repositoryMock
            .Setup(r => r.TrySave(It.IsAny<Reservation>()))
            .Returns(true);

        var service = new ReservationService(
            availability,
            repositoryMock.Object,
            spy
        );

        // Act
        bool result = service.Reserve("test@example.com", "EVENT1", 2);

        // Assert
        Assert.True(result);
        Assert.Single(spy.SentReservations);

        repositoryMock.Verify(
            r => r.TrySave(It.Is<Reservation>(reservation =>
                reservation.CustomerEmail == "test@example.com" &&
                reservation.EventCode == "EVENT1" &&
                reservation.SeatCount == 2
            )),
            Times.Once()
        );
    }

    [Fact]
    public void Reserve_ZuWenigPlaetze_SpeichertNicht()
    {
        // Arrange
        var availability = new SeatAvailabilityStub(1);
        var repositoryMock = new Mock<IReservationRepository>();
        var spy = new ConfirmationSpy();

        repositoryMock
            .Setup(r => r.TrySave(It.IsAny<Reservation>()))
            .Returns(true);

        var service = new ReservationService(
            availability,
            repositoryMock.Object,
            spy
        );

        // Act
        bool result = service.Reserve("test@example.com", "EVENT1", 2);

        // Assert
        Assert.False(result);
        Assert.Empty(spy.SentReservations);

        repositoryMock.Verify(
            r => r.TrySave(It.IsAny<Reservation>()),
            Times.Never()
        );
    }

    [Fact]
    public void Reserve_SpeichernFehlgeschlagen_ReturnsFalse()
    {
        // Arrange
        var availability = new SeatAvailabilityStub(3);
        var repositoryMock = new Mock<IReservationRepository>();
        var spy = new ConfirmationSpy();

        repositoryMock
            .Setup(r => r.TrySave(It.IsAny<Reservation>()))
            .Returns(false);

        var service = new ReservationService(
            availability,
            repositoryMock.Object,
            spy
        );

        // Act
        bool result = service.Reserve("test@example.com", "EVENT1", 2);

        // Assert
        Assert.False(result);
        Assert.Empty(spy.SentReservations);

        repositoryMock.Verify(
            r => r.TrySave(It.Is<Reservation>(reservation =>
                reservation.CustomerEmail == "test@example.com" &&
                reservation.EventCode == "EVENT1" &&
                reservation.SeatCount == 2
            )),
            Times.Once()
        );
    }
}
