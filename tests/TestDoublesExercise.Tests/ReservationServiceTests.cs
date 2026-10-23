namespace TestDoublesExercise.Tests;

// TODO Aufgabe 1: Kurze Antwort.
// Welche Aussagen erlaubt die Schnittstelle ISeatAvailability bereits (ohne
// produktive Implementierung)? Welche fachliche Annahme trefft ihr für
// GetAvailableSeats in euren eigenen Stubs?

// TODO Aufgabe 3: Kurze Antwort.
// Erklärt in einem Satz den Unterschied zwischen eurem Stub und eurem Spy.

// TODO Aufgabe 5: Kurze Antwort.
// Begründet für jede Abhängigkeit, ob ihr ein handgeschriebenes Double oder
// Moq verwendet habt.

public sealed class ReservationServiceTests
{
    // Aufgabe 1: Eigener ISeatAvailability-Stub.
    // Arrange: Stub mit fester Platzzahl für einen Veranstaltungscode,
    //          eigenes IReservationRepository (TrySave liefert immer true),
    //          eigenen IConfirmationSender (Send tut nichts).
    // Act: service.Reserve(...) aufrufen.
    // Assert: Reserve liefert true, wenn genug Plätze verfügbar sind und das
    //         Speichern gelingt.
    [Fact(Skip = "TODO: Aufgabe 1 implementieren und Skip entfernen.")]
    public void Reserve_DreiVerfuegbarZweiAngefragt_GibtTrueZurueck()
    {
    }

    // Arrange: Stub mit 1 verfügbarem Platz, 2 angefragt.
    // Act: service.Reserve(...) aufrufen.
    // Assert: Reserve liefert false.
    [Fact(Skip = "TODO: Aufgabe 1 implementieren und Skip entfernen.")]
    public void Reserve_EinVerfuegbarZweiAngefragt_GibtFalseZurueck()
    {
    }

    // Arrange: Stub mit genau 2 verfügbaren Plätzen, 2 angefragt.
    // Act: service.Reserve(...) aufrufen.
    // Assert: Reservierung ist möglich (Reserve liefert true).
    [Fact(Skip = "TODO: Aufgabe 1 implementieren und Skip entfernen.")]
    public void Reserve_GenauZweiVerfuegbarZweiAngefragt_GibtTrueZurueck()
    {
    }

    // Aufgabe 2: Grenzfälle und Isolation.
    // Arrange: Doubles, die sichtbar machen, ob sie aufgerufen wurden (z. B.
    //          über eine Zähler- oder Calls-Eigenschaft im eigenen Double).
    // Act: service.Reserve(...) mit seatCount 0 aufrufen.
    // Assert: Erwarteter Exception-Typ wird geworfen und keine Abhängigkeit
    //         wurde aufgerufen.
    [Fact(Skip = "TODO: Aufgabe 2 implementieren und Skip entfernen.")]
    public void Reserve_SeatCountNull_WirftExceptionOhneAbhaengigkeitenAufzurufen()
    {
    }

    // Act: service.Reserve(...) mit seatCount -1 aufrufen.
    // Assert: Erwarteter Exception-Typ, keine Abhängigkeit aufgerufen.
    [Fact(Skip = "TODO: Aufgabe 2 implementieren und Skip entfernen.")]
    public void Reserve_SeatCountNegativ_WirftExceptionOhneAbhaengigkeitenAufzurufen()
    {
    }

    // Act: service.Reserve(...) mit leerem Veranstaltungscode aufrufen.
    // Assert: Erwarteter Exception-Typ, keine Abhängigkeit aufgerufen.
    [Fact(Skip = "TODO: Aufgabe 2 implementieren und Skip entfernen.")]
    public void Reserve_LeererVeranstaltungscode_WirftExceptionOhneAbhaengigkeitenAufzurufen()
    {
    }

    // Aufgabe 3: Eigener IConfirmationSender-Spy.
    // Arrange: Spy, der alle übergebenen Reservation-Objekte in Reihenfolge
    //          aufzeichnet.
    // Act: service.Reserve(...) mit ausreichenden Plätzen aufrufen.
    // Assert: Genau eine Bestätigung mit E-Mail, Veranstaltungscode und
    //         Platzanzahl wurde versendet.
    [Fact(Skip = "TODO: Aufgabe 3 implementieren und Skip entfernen.")]
    public void Reserve_ErfolgreichesSpeichern_VersendetGenauEineBestaetigung()
    {
    }

    // Act: service.Reserve(...) mit zu wenigen Plätzen aufrufen.
    // Assert: Keine Bestätigung wurde versendet.
    [Fact(Skip = "TODO: Aufgabe 3 implementieren und Skip entfernen.")]
    public void Reserve_ZuWenigPlaetze_VersendetKeineBestaetigung()
    {
    }

    // Arrange: Repository-Double, dessen TrySave false liefert.
    // Act: service.Reserve(...) aufrufen.
    // Assert: Keine Bestätigung wurde versendet.
    [Fact(Skip = "TODO: Aufgabe 3 implementieren und Skip entfernen.")]
    public void Reserve_TrySaveLiefertFalse_VersendetKeineBestaetigung()
    {
    }

    // Aufgabe 4: Moq für IReservationRepository.
    // Arrange: Mock<IReservationRepository> mit Setup(...).Returns(true).
    // Act: service.Reserve(...) mit ausreichenden Plätzen aufrufen.
    // Assert: Verify(..., Times.Once()) mit der erwarteten Reservierung
    //         (Eigenschaften prüfen, nicht nur It.IsAny<Reservation>()),
    //         zusätzlich Rückgabewert und Spy kontrollieren.
    [Fact(Skip = "TODO: Aufgabe 4 implementieren und Skip entfernen.")]
    public void Reserve_MitMoqRepository_SpeichertErwarteteReservierungGenauEinmal()
    {
    }

    // Arrange: Mock<IReservationRepository>, Stub meldet zu wenige Plätze.
    // Act: service.Reserve(...) aufrufen.
    // Assert: Verify(..., Times.Never()), dass TrySave nicht aufgerufen wird.
    [Fact(Skip = "TODO: Aufgabe 4 implementieren und Skip entfernen.")]
    public void Reserve_ZuWenigPlaetzeMitMoqRepository_RuftTrySaveNieAuf()
    {
    }

    // Aufgabe 5: E-Mail-Adresse besteht nur aus Leerzeichen.
    // Arrange: Doubles eurer Wahl (handgeschrieben oder Moq, begründet in
    //          der Antwort oben).
    // Act: service.Reserve(...) mit Leerzeichen-E-Mail aufrufen.
    // Assert: Erwarteter Exception-Typ; kein Speichern, keine Bestätigung.
    [Fact(Skip = "TODO: Aufgabe 5 implementieren und Skip entfernen.")]
    public void Reserve_EmailNurLeerzeichen_WirftExceptionOhneSpeichernUndBestaetigung()
    {
    }
}
