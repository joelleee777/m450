
using Xunit;
using Moq;
using TestDoublesExercise;

namespace TestDoublesExercise.Tests;

public class ReservationEmailTests
{
    [Fact]
    public void Reserve_EmailNurLeerzeichen_WirftException()
    {
        // Arrange
        var availability = new SeatAvailabilityStub(3);
        var repositoryMock = new Mock<IReservationRepository>();
        var spy = new ConfirmationSpy();

        var service = new ReservationService(
            availability,
            repositoryMock.Object,
            spy
        );

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            service.Reserve("   ", "EVENT1", 2)
        );

        repositoryMock.Verify(
            r => r.TrySave(It.IsAny<Reservation>()),
            Times.Never()
        );

        Assert.Empty(spy.SentReservations);
    }
}
