namespace TestDoublesExercise.MSTest.Tests;

// Eigene, handgeschriebene Doubles für ReservationService - verwendet in
// ReservationServiceTests.cs und ReservationBatchTests.cs.

internal sealed class SeatAvailabilityStub : ISeatAvailability
{
    private readonly Dictionary<string, int> _seatsByEvent;

    public SeatAvailabilityStub(string eventCode, int availableSeats)
        : this(new Dictionary<string, int> { [eventCode] = availableSeats })
    {
    }

    public SeatAvailabilityStub(Dictionary<string, int> seatsByEvent)
    {
        _seatsByEvent = seatsByEvent;
    }

    public List<string> QueriedEventCodes { get; } = [];

    public int GetAvailableSeats(string eventCode)
    {
        QueriedEventCodes.Add(eventCode);
        return _seatsByEvent.GetValueOrDefault(eventCode);
    }
}

internal sealed class ReservationRepositoryStub(bool saveSucceeds = true) : IReservationRepository
{
    public int SaveCallCount { get; private set; }

    public IReadOnlyList<Reservation>? SavedBatch { get; private set; }

    public bool TrySave(Reservation reservation)
    {
        SaveCallCount++;
        return saveSucceeds;
    }

    public bool TrySaveBatch(IReadOnlyList<Reservation> reservations)
    {
        SavedBatch = reservations;
        return saveSucceeds;
    }
}

internal sealed class ConfirmationSenderSpy : IConfirmationSender
{
    public List<Reservation> SentConfirmations { get; } = [];

    public void Send(Reservation reservation) => SentConfirmations.Add(reservation);
}
