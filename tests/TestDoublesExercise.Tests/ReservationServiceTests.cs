using Moq;

namespace TestDoublesExercise.Tests;

// Aufgabe 1, Antwort: ISeatAvailability legt nur Signatur und Vertrag fest -
// GetAvailableSeats(eventCode) liefert eine int-Anzahl freier Plätze für
// einen Code. Wie das intern berechnet wird (Datenbank, Cache, ...), ist
// unbekannt. Fachliche Annahme für den Stub: Für einen bekannten Code
// liefert die Methode eine feste, nicht-negative Anzahl freier Plätze;
// unbekannte Codes liefern 0.

public sealed class ReservationServiceTests
{
    // --- Aufgabe 1: eigener ISeatAvailability-Stub ---

    [Fact]
    public void Reserve_DreiPlaetzeFreiZweiAngefragt_GibtTrueZurueck()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 3);
        var service = new ReservationService(availability, new ReservationRepositoryStub(), new ConfirmationSenderSpy());

        // Act
        var result = service.Reserve("ana@example.com", "EVT-1", 2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Reserve_EinPlatzFreiZweiAngefragt_GibtFalseZurueck()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 1);
        var service = new ReservationService(availability, new ReservationRepositoryStub(), new ConfirmationSenderSpy());

        // Act
        var result = service.Reserve("ana@example.com", "EVT-1", 2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Reserve_GenauZweiPlaetzeFreiZweiAngefragt_GibtTrueZurueck()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 2);
        var service = new ReservationService(availability, new ReservationRepositoryStub(), new ConfirmationSenderSpy());

        // Act
        var result = service.Reserve("ana@example.com", "EVT-1", 2);

        // Assert
        Assert.True(result);
    }

    // --- Aufgabe 2: Grenzfälle und Isolation ---

    [Fact]
    public void Reserve_SeatCountNull_WirftExceptionOhneAbhaengigkeitAufzurufen()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 5);
        var repository = new ReservationRepositoryStub();
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);

        // Act
        Assert.Throws<ArgumentOutOfRangeException>(() => service.Reserve("ana@example.com", "EVT-1", 0));

        // Assert
        Assert.Empty(availability.QueriedEventCodes);
        Assert.Equal(0, repository.SaveCallCount);
        Assert.Empty(sender.SentConfirmations);
    }

    [Fact]
    public void Reserve_SeatCountNegativ_WirftExceptionOhneAbhaengigkeitAufzurufen()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 5);
        var repository = new ReservationRepositoryStub();
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);

        // Act
        Assert.Throws<ArgumentOutOfRangeException>(() => service.Reserve("ana@example.com", "EVT-1", -1));

        // Assert
        Assert.Empty(availability.QueriedEventCodes);
        Assert.Equal(0, repository.SaveCallCount);
        Assert.Empty(sender.SentConfirmations);
    }

    [Fact]
    public void Reserve_LeererEventCode_WirftExceptionOhneAbhaengigkeitAufzurufen()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 5);
        var repository = new ReservationRepositoryStub();
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);

        // Act
        Assert.Throws<ArgumentException>(() => service.Reserve("ana@example.com", "", 1));

        // Assert
        Assert.Empty(availability.QueriedEventCodes);
        Assert.Equal(0, repository.SaveCallCount);
        Assert.Empty(sender.SentConfirmations);
    }

    // --- Aufgabe 3: eigener IConfirmationSender-Spy ---
    // Antwort: Ein Stub beantwortet Fragen - er liefert kontrollierte Werte.
    // Ein Spy merkt sich, was passiert ist - Aufrufe und Argumente -, damit
    // der Test es im Assert-Teil nachträglich prüfen kann.

    [Fact]
    public void Reserve_ErfolgreicheReservierung_SpyZeichnetGenauEineBestaetigungAuf()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 3);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, new ReservationRepositoryStub(), sender);

        // Act
        service.Reserve("ana@example.com", "EVT-1", 2);

        // Assert
        var confirmation = Assert.Single(sender.SentConfirmations);
        Assert.Equal("ana@example.com", confirmation.CustomerEmail);
        Assert.Equal("EVT-1", confirmation.EventCode);
        Assert.Equal(2, confirmation.SeatCount);
    }

    [Fact]
    public void Reserve_ZuWenigPlaetze_SpyBleibtLeer()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 1);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, new ReservationRepositoryStub(), sender);

        // Act
        var result = service.Reserve("ana@example.com", "EVT-1", 2);

        // Assert
        Assert.False(result);
        Assert.Empty(sender.SentConfirmations);
    }

    [Fact]
    public void Reserve_TrySaveLiefertFalse_SpyBleibtLeer()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 3);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, new ReservationRepositoryStub(saveSucceeds: false), sender);

        // Act
        var result = service.Reserve("ana@example.com", "EVT-1", 2);

        // Assert
        Assert.False(result);
        Assert.Empty(sender.SentConfirmations);
    }

    // --- Aufgabe 4: Moq für das Repository ---

    [Fact]
    public void Reserve_MitMockRepository_SpeichertDieRichtigeReservierungGenauEinmal()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 3);
        var sender = new ConfirmationSenderSpy();
        var repositoryMock = new Mock<IReservationRepository>();
        repositoryMock.Setup(r => r.TrySave(It.IsAny<Reservation>())).Returns(true);
        var service = new ReservationService(availability, repositoryMock.Object, sender);

        // Act
        var result = service.Reserve("ana@example.com", "EVT-1", 2);

        // Assert
        Assert.True(result);
        repositoryMock.Verify(
            r => r.TrySave(It.Is<Reservation>(reservation =>
                reservation.CustomerEmail == "ana@example.com" &&
                reservation.EventCode == "EVT-1" &&
                reservation.SeatCount == 2)),
            Times.Once());
        Assert.Single(sender.SentConfirmations);
    }

    [Fact]
    public void Reserve_ZuWenigPlaetzeMitMockRepository_RuftTrySaveNieAuf()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 1);
        var sender = new ConfirmationSenderSpy();
        var repositoryMock = new Mock<IReservationRepository>();
        var service = new ReservationService(availability, repositoryMock.Object, sender);

        // Act
        var result = service.Reserve("ana@example.com", "EVT-1", 2);

        // Assert
        Assert.False(result);
        repositoryMock.Verify(r => r.TrySave(It.IsAny<Reservation>()), Times.Never());
        Assert.Empty(sender.SentConfirmations);
    }

    // --- Aufgabe 5: handgeschrieben oder Moq? ---
    // Antwort: Für alle drei Abhängigkeiten reicht hier ein einfaches
    // handgeschriebenes Double. Der Stub muss nichts Komplexes liefern, und
    // bei Repository und Sender wird nur geprüft, dass sie NICHT aufgerufen
    // werden - dafür braucht es keine Moq-Konfiguration, ein Zähler im
    // eigenen Double reicht.

    [Fact]
    public void Reserve_EmailNurLeerzeichen_WirftExceptionOhneSpeichernUndBestaetigung()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 5);
        var repository = new ReservationRepositoryStub();
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);

        // Act
        Assert.Throws<ArgumentException>(() => service.Reserve("   ", "EVT-1", 2));

        // Assert
        Assert.Equal(0, repository.SaveCallCount);
        Assert.Empty(sender.SentConfirmations);
    }
}
