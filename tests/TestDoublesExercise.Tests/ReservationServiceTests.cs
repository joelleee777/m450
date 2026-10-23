namespace TestDoublesExercise.Tests;

public sealed class ReservationServiceTests
{
    // Entfernt Skip erst, wenn der jeweilige Test fertig implementiert ist.
    // Beispiele für Stub, Spy und Moq: TestDoubleExamples.cs.

    // --- Aufgabe 1: eigener ISeatAvailability-Stub ---

    [Fact(Skip = "TODO: eigenen ISeatAvailability-Stub schreiben")]
    public void Reserve_DreiPlaetzeFreiZweiAngefragt_GibtTrueZurueck()
    {
        // Arrange: Stub mit 3 freien Plätzen, Repository speichert erfolgreich, eigener Spy.
        // Act:     2 Plätze reservieren.
        // Assert:  true.
        throw new NotImplementedException();
    }

    [Fact(Skip = "TODO: zu wenige Plätze testen")]
    public void Reserve_EinPlatzFreiZweiAngefragt_GibtFalseZurueck()
    {
        // Arrange: Stub mit 1 freiem Platz.
        // Act:     2 Plätze reservieren.
        // Assert:  false.
        throw new NotImplementedException();
    }

    [Fact(Skip = "TODO: Grenzfall testen")]
    public void Reserve_GenauZweiPlaetzeFreiZweiAngefragt_GibtTrueZurueck()
    {
        // Arrange: Stub mit genau 2 freien Plätzen.
        // Act:     2 Plätze reservieren.
        // Assert:  true.
        throw new NotImplementedException();
    }

    // --- Aufgabe 2: Grenzfälle und Isolation ---

    [Fact(Skip = "TODO: seatCount 0 testen")]
    public void Reserve_SeatCountNull_WirftExceptionOhneAbhaengigkeitAufzurufen()
    {
        // Arrange: Doubles, deren Aufrufe sich zählen lassen.
        // Act:     seatCount 0.
        // Assert:  ArgumentOutOfRangeException, keine Abhängigkeit aufgerufen.
        throw new NotImplementedException();
    }

    [Fact(Skip = "TODO: seatCount -1 testen")]
    public void Reserve_SeatCountNegativ_WirftExceptionOhneAbhaengigkeitAufzurufen()
    {
        // Arrange: Doubles, deren Aufrufe sich zählen lassen.
        // Act:     seatCount -1.
        // Assert:  ArgumentOutOfRangeException, keine Abhängigkeit aufgerufen.
        throw new NotImplementedException();
    }

    [Fact(Skip = "TODO: leeren Veranstaltungscode testen")]
    public void Reserve_LeererEventCode_WirftExceptionOhneAbhaengigkeitAufzurufen()
    {
        // Arrange: Doubles, deren Aufrufe sich zählen lassen.
        // Act:     eventCode = "".
        // Assert:  ArgumentException, keine Abhängigkeit aufgerufen.
        throw new NotImplementedException();
    }

    // --- Aufgabe 3: eigener IConfirmationSender-Spy ---

    [Fact(Skip = "TODO: eigenen IConfirmationSender-Spy schreiben")]
    public void Reserve_ErfolgreicheReservierung_SpyZeichnetGenauEineBestaetigungAuf()
    {
        // Arrange: genug Plätze, Repository speichert erfolgreich, eigener Spy.
        // Act:     Reservierung durchführen.
        // Assert:  genau eine aufgezeichnete Bestätigung mit Email, EventCode, SeatCount.
        throw new NotImplementedException();
    }

    [Fact(Skip = "TODO: zu wenige Plätze mit Spy testen")]
    public void Reserve_ZuWenigPlaetze_SpyBleibtLeer()
    {
        // Arrange: zu wenige Plätze, eigener Spy.
        // Act:     Reservierung versuchen.
        // Assert:  Spy hat nichts aufgezeichnet.
        throw new NotImplementedException();
    }

    [Fact(Skip = "TODO: fehlgeschlagenes Speichern mit Spy testen")]
    public void Reserve_TrySaveLiefertFalse_SpyBleibtLeer()
    {
        // Arrange: Repository-Double mit TrySave -> false, eigener Spy.
        // Act:     Reservierung versuchen.
        // Assert:  false; Spy hat nichts aufgezeichnet.
        throw new NotImplementedException();
    }

    // --- Aufgabe 4: Moq für das Repository ---

    [Fact(Skip = "TODO: Moq-Repository mit erfolgreichem Speichern einsetzen")]
    public void Reserve_MitMockRepository_SpeichertDieRichtigeReservierungGenauEinmal()
    {
        // Arrange: Mock<IReservationRepository>, Setup liefert true.
        // Act:     Reservierung durchführen.
        // Assert:  Verify(..., Times.Once()) mit den erwarteten Reservation-Werten, plus Rückgabewert und Spy.
        throw new NotImplementedException();
    }

    [Fact(Skip = "TODO: Moq-Repository bei zu wenigen Plätzen einsetzen")]
    public void Reserve_ZuWenigPlaetzeMitMockRepository_RuftTrySaveNieAuf()
    {
        // Arrange: Stub meldet zu wenige Plätze, Mock<IReservationRepository>.
        // Act:     Reservierung versuchen.
        // Assert:  Verify(..., Times.Never()) für TrySave.
        throw new NotImplementedException();
    }

    // --- Aufgabe 5: handgeschrieben oder Moq? ---

    [Fact(Skip = "TODO: E-Mail aus Leerzeichen testen")]
    public void Reserve_EmailNurLeerzeichen_WirftExceptionOhneSpeichernUndBestaetigung()
    {
        // Arrange: Doubles eurer Wahl, begründet in eurer Antwort.
        // Act:     customerEmail = "   ".
        // Assert:  ArgumentException; kein Speichern, keine Bestätigung.
        throw new NotImplementedException();
    }

    // --- Fortgeschrittene Aufgabe: Gruppenreservierung ---

    [Fact(Skip = "Fortgeschrittene Aufgabe: ReserveBatch zuerst implementieren")]
    public void ReserveBatch_ZweiReservierungenUeberschreitenGemeinsamDieKapazitaet_GibtFalseZurueck()
    {
        // Arrange: zwei Reservierungen für dasselbe Event, die einzeln passen, zusammen aber nicht.
        // Act:     ReserveBatch mit beiden Einträgen aufrufen.
        // Assert:  false; kein Batch-Speichern, keine Bestätigungen.
        throw new NotImplementedException();
    }
}
