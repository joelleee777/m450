using Moq;

namespace TestDoublesExercise.MSTest.Tests;

// Fortgeschrittene Aufgabe: Gruppenreservierung (ReserveBatch).
//
// Notiz zum Fehlerfall: Wirft Send nach erfolgreichem Speichern eine
// Exception, sind die Reservierungen bereits dauerhaft gespeichert, aber
// der Kunde hat womöglich keine Bestätigung erhalten - ein inkonsistenter
// Zustand. Mögliche fachliche Lösung: den Batch als "gespeichert, Bestätigung
// ausstehend" markieren und das Versenden separat mit Wiederholungen
// nachholen, statt die ganze Reservierung rückgängig zu machen.

[TestClass]
public sealed class ReservationBatchTests
{
    [TestMethod]
    public void ReserveBatch_RequestsIstNull_WirftException()
    {
        // Arrange
        var availability = new SeatAvailabilityStub([]);
        var service = new ReservationService(availability, new ReservationRepositoryStub(), new ConfirmationSenderSpy());

        // Act
        Assert.ThrowsExactly<ArgumentNullException>(() => service.ReserveBatch(null!));

        // Assert
        Assert.AreEqual(0, availability.QueriedEventCodes.Count);
    }

    [TestMethod]
    public void ReserveBatch_LeereListe_WirftException()
    {
        // Arrange
        var availability = new SeatAvailabilityStub([]);
        var service = new ReservationService(availability, new ReservationRepositoryStub(), new ConfirmationSenderSpy());

        // Act
        Assert.ThrowsExactly<ArgumentException>(() => service.ReserveBatch([]));

        // Assert
        Assert.AreEqual(0, availability.QueriedEventCodes.Count);
    }

    [TestMethod]
    public void ReserveBatch_EintragMitLeererEmail_WirftExceptionOhneAbhaengigkeitAufzurufen()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 5);
        var repository = new ReservationRepositoryStub();
        Reservation[] requests = [new Reservation("   ", "EVT-1", 1)];
        var service = new ReservationService(availability, repository, new ConfirmationSenderSpy());

        // Act
        Assert.ThrowsExactly<ArgumentException>(() => service.ReserveBatch(requests));

        // Assert
        Assert.AreEqual(0, availability.QueriedEventCodes.Count);
        Assert.AreEqual(0, repository.SaveCallCount);
    }

    [TestMethod]
    public void ReserveBatch_EintragMitSeatCountNull_WirftExceptionOhneAbhaengigkeitAufzurufen()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 5);
        Reservation[] requests = [new Reservation("ana@example.com", "EVT-1", 0)];
        var service = new ReservationService(availability, new ReservationRepositoryStub(), new ConfirmationSenderSpy());

        // Act
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => service.ReserveBatch(requests));

        // Assert
        Assert.AreEqual(0, availability.QueriedEventCodes.Count);
    }

    [TestMethod]
    public void ReserveBatch_MehrereEintraegeGleichesEvent_FragtVerfuegbarkeitGenauEinmalAb()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 5);
        Reservation[] requests =
        [
            new Reservation("ana@example.com", "EVT-1", 2),
            new Reservation("bo@example.com", "EVT-1", 2),
        ];
        var service = new ReservationService(availability, new ReservationRepositoryStub(), new ConfirmationSenderSpy());

        // Act
        var result = service.ReserveBatch(requests);

        // Assert
        Assert.IsTrue(result);
        CollectionAssert.AreEqual(new List<string> { "EVT-1" }, availability.QueriedEventCodes);
    }

    [TestMethod]
    public void ReserveBatch_ZweiReservierungenUeberschreitenGemeinsamDieKapazitaet_GibtFalseZurueck()
    {
        // Arrange: beide passen einzeln (3 <= 5), zusammen aber nicht (6 > 5).
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 5);
        var repository = new ReservationRepositoryStub();
        var sender = new ConfirmationSenderSpy();
        Reservation[] requests =
        [
            new Reservation("ana@example.com", "EVT-1", 3),
            new Reservation("bo@example.com", "EVT-1", 3),
        ];
        var service = new ReservationService(availability, repository, sender);

        // Act
        var result = service.ReserveBatch(requests);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(0, repository.SaveCallCount);
        Assert.IsNull(repository.SavedBatch);
        Assert.AreEqual(0, sender.SentConfirmations.Count);
    }

    [TestMethod]
    public void ReserveBatch_TrySaveBatchLiefertFalse_VersendetKeineBestaetigungen()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 5);
        var sender = new ConfirmationSenderSpy();
        Reservation[] requests = [new Reservation("ana@example.com", "EVT-1", 2)];
        var service = new ReservationService(availability, new ReservationRepositoryStub(saveSucceeds: false), sender);

        // Act
        var result = service.ReserveBatch(requests);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(0, sender.SentConfirmations.Count);
    }

    [TestMethod]
    public void ReserveBatch_ErfolgreichesSpeichern_VersendetBestaetigungenInEingabeReihenfolge()
    {
        // Arrange
        var availability = new SeatAvailabilityStub(new Dictionary<string, int> { ["EVT-1"] = 5, ["EVT-2"] = 5 });
        var sender = new ConfirmationSenderSpy();
        Reservation[] requests =
        [
            new Reservation("ana@example.com", "EVT-1", 2),
            new Reservation("bo@example.com", "EVT-2", 1),
        ];
        var service = new ReservationService(availability, new ReservationRepositoryStub(), sender);

        // Act
        var result = service.ReserveBatch(requests);

        // Assert
        Assert.IsTrue(result);
        CollectionAssert.AreEqual(requests, sender.SentConfirmations);
    }

    [TestMethod]
    public void ReserveBatch_MitMockRepository_SpeichertAlleReservierungenInEinemAufruf()
    {
        // Arrange
        var availability = new SeatAvailabilityStub("EVT-1", availableSeats: 5);
        var sender = new ConfirmationSenderSpy();
        var repositoryMock = new Mock<IReservationRepository>();
        repositoryMock.Setup(r => r.TrySaveBatch(It.IsAny<IReadOnlyList<Reservation>>())).Returns(true);
        Reservation[] requests =
        [
            new Reservation("ana@example.com", "EVT-1", 2),
            new Reservation("bo@example.com", "EVT-1", 1),
        ];
        var service = new ReservationService(availability, repositoryMock.Object, sender);

        // Act
        var result = service.ReserveBatch(requests);

        // Assert
        Assert.IsTrue(result);
        repositoryMock.Verify(
            r => r.TrySaveBatch(It.Is<IReadOnlyList<Reservation>>(batch =>
                batch.Count == 2 &&
                batch[0].CustomerEmail == "ana@example.com" &&
                batch[1].CustomerEmail == "bo@example.com")),
            Times.Once());
        Assert.AreEqual(2, sender.SentConfirmations.Count);
    }
}
