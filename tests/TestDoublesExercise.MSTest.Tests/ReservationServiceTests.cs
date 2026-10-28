using Moq;

namespace TestDoublesExercise.MSTest.Tests;

// Aufgabe 1, Antwort: ISeatAvailability legt nur Signatur und Vertrag fest -
// GetAvailableSeats(eventCode) liefert eine int-Anzahl freier Plätze für
// einen Code. Wie das intern berechnet wird (Datenbank, Cache, ...), ist
// unbekannt. Fachliche Annahme für den Stub: Für einen bekannten Code
// liefert die Methode eine feste, nicht-negative Anzahl freier Plätze;
// unbekannte Codes liefern 0.

[TestClass]
public sealed class ReservationServiceTests
{
    // --- Aufgabe 1: eigener ISeatAvailability-Stub ---

    [TestMethod]
    public void Reserve_DreiPlaetzeFreiZweiAngefragt_GibtTrueZurueck()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 3);
        var service = new ReservationService(availability, new ReservationRepositoryStub(), new ConfirmationSenderSpy());

        // Act
        var result = service.Reserve("ana@example.com", "EVT-1", 2);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Reserve_EinPlatzFreiZweiAngefragt_GibtFalseZurueck()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 1);
        var service = new ReservationService(availability, new ReservationRepositoryStub(), new ConfirmationSenderSpy());

        // Act
        var result = service.Reserve("ana@example.com", "EVT-1", 2);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Reserve_GenauZweiPlaetzeFreiZweiAngefragt_GibtTrueZurueck()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 2);
        var service = new ReservationService(availability, new ReservationRepositoryStub(), new ConfirmationSenderSpy());

        // Act
        var result = service.Reserve("ana@example.com", "EVT-1", 2);

        // Assert
        Assert.IsTrue(result);
    }

    // --- Aufgabe 2: Grenzfälle und Isolation ---

    [TestMethod]
    public void Reserve_SeatCountNull_WirftExceptionOhneAbhaengigkeitAufzurufen()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 5);
        var repository = new ReservationRepositoryStub();
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => service.Reserve("ana@example.com", "EVT-1", 0));

        // Assert
        Assert.AreEqual(0, availability.QueriedEventCodes.Count);
        Assert.AreEqual(0, repository.SaveCallCount);
        Assert.AreEqual(0, sender.SentConfirmations.Count);
    }

    [TestMethod]
    public void Reserve_SeatCountNegativ_WirftExceptionOhneAbhaengigkeitAufzurufen()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 5);
        var repository = new ReservationRepositoryStub();
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => service.Reserve("ana@example.com", "EVT-1", -1));

        // Assert
        Assert.AreEqual(0, availability.QueriedEventCodes.Count);
        Assert.AreEqual(0, repository.SaveCallCount);
        Assert.AreEqual(0, sender.SentConfirmations.Count);
    }

    [TestMethod]
    public void Reserve_LeererEventCode_WirftExceptionOhneAbhaengigkeitAufzurufen()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 5);
        var repository = new ReservationRepositoryStub();
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);

        // Act
        Assert.ThrowsExactly<ArgumentException>(() => service.Reserve("ana@example.com", "", 1));

        // Assert
        Assert.AreEqual(0, availability.QueriedEventCodes.Count);
        Assert.AreEqual(0, repository.SaveCallCount);
        Assert.AreEqual(0, sender.SentConfirmations.Count);
    }

    // --- Aufgabe 3: eigener IConfirmationSender-Spy ---
    // Antwort: Ein Stub beantwortet Fragen - er liefert kontrollierte Werte.
    // Ein Spy merkt sich, was passiert ist - Aufrufe und Argumente -, damit
    // der Test es im Assert-Teil nachträglich prüfen kann.

    [TestMethod]
    public void Reserve_ErfolgreicheReservierung_SpyZeichnetGenauEineBestaetigungAuf()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 3);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, new ReservationRepositoryStub(), sender);

        // Act
        service.Reserve("ana@example.com", "EVT-1", 2);

        // Assert
        Assert.AreEqual(1, sender.SentConfirmations.Count);
        var confirmation = sender.SentConfirmations[0];
        Assert.AreEqual("ana@example.com", confirmation.CustomerEmail);
        Assert.AreEqual("EVT-1", confirmation.EventCode);
        Assert.AreEqual(2, confirmation.SeatCount);
    }

    [TestMethod]
    public void Reserve_ZuWenigPlaetze_SpyBleibtLeer()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 1);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, new ReservationRepositoryStub(), sender);

        // Act
        var result = service.Reserve("ana@example.com", "EVT-1", 2);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(0, sender.SentConfirmations.Count);
    }

    [TestMethod]
    public void Reserve_TrySaveLiefertFalse_SpyBleibtLeer()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 3);
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, new ReservationRepositoryStub(saveSucceeds: false), sender);

        // Act
        var result = service.Reserve("ana@example.com", "EVT-1", 2);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(0, sender.SentConfirmations.Count);
    }

    // --- Aufgabe 4: Moq für das Repository ---

    [TestMethod]
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
        Assert.IsTrue(result);
        repositoryMock.Verify(
            r => r.TrySave(It.Is<Reservation>(reservation =>
                reservation.CustomerEmail == "ana@example.com" &&
                reservation.EventCode == "EVT-1" &&
                reservation.SeatCount == 2)),
            Times.Once());
        Assert.AreEqual(1, sender.SentConfirmations.Count);
    }

    [TestMethod]
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
        Assert.IsFalse(result);
        repositoryMock.Verify(r => r.TrySave(It.IsAny<Reservation>()), Times.Never());
        Assert.AreEqual(0, sender.SentConfirmations.Count);
    }

    // --- Aufgabe 5: handgeschrieben oder Moq? ---
    // Antwort: Für alle drei Abhängigkeiten reicht hier ein einfaches
    // handgeschriebenes Double. Der Stub muss nichts Komplexes liefern, und
    // bei Repository und Sender wird nur geprüft, dass sie NICHT aufgerufen
    // werden - dafür braucht es keine Moq-Konfiguration, ein Zähler im
    // eigenen Double reicht.

    [TestMethod]
    public void Reserve_EmailNurLeerzeichen_WirftExceptionOhneSpeichernUndBestaetigung()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 5);
        var repository = new ReservationRepositoryStub();
        var sender = new ConfirmationSenderSpy();
        var service = new ReservationService(availability, repository, sender);

        // Act
        Assert.ThrowsExactly<ArgumentException>(() => service.Reserve("   ", "EVT-1", 2));

        // Assert
        Assert.AreEqual(0, repository.SaveCallCount);
        Assert.AreEqual(0, sender.SentConfirmations.Count);
    }
}
