namespace TestDoublesExercise;

public sealed record Reservation(string CustomerEmail, string EventCode, int SeatCount);

// Produktive Implementierung fehlt noch - nur die Schnittstelle ist bekannt.
public interface ISeatAvailability
{
    int GetAvailableSeats(string eventCode);
}

public interface IReservationRepository
{
    bool TrySave(Reservation reservation);

    // Speichert alle Reservierungen atomar: entweder alle oder keine.
    bool TrySaveBatch(IReadOnlyList<Reservation> reservations);
}

public interface IConfirmationSender
{
    void Send(Reservation reservation);
}

public sealed class ReservationService
{
    private readonly ISeatAvailability _availability;
    private readonly IReservationRepository _repository;
    private readonly IConfirmationSender _sender;

    public ReservationService(
        ISeatAvailability availability,
        IReservationRepository repository,
        IConfirmationSender sender)
    {
        _availability = availability ?? throw new ArgumentNullException(nameof(availability));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    public bool Reserve(string customerEmail, string eventCode, int seatCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(seatCount);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(eventCode);

        var freeSeats = _availability.GetAvailableSeats(eventCode);
        if (freeSeats < seatCount)
        {
            return false;
        }

        var reservation = new Reservation(customerEmail, eventCode, seatCount);
        if (!_repository.TrySave(reservation))
        {
            return false;
        }

        _sender.Send(reservation);
        return true;
    }

    public bool ReserveBatch(IReadOnlyList<Reservation> requests)
    {
        ArgumentNullException.ThrowIfNull(requests);
        if (requests.Count == 0)
        {
            throw new ArgumentException("Mindestens eine Reservierung ist erforderlich.", nameof(requests));
        }

        foreach (var request in requests)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.CustomerEmail);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.EventCode);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(request.SeatCount);
        }

        var requestedSeatsByEvent = requests
            .GroupBy(request => request.EventCode)
            .ToDictionary(group => group.Key, group => group.Sum(request => request.SeatCount));

        var hasEnoughSeatsEverywhere = true;
        foreach (var (eventCode, requestedSeats) in requestedSeatsByEvent)
        {
            if (_availability.GetAvailableSeats(eventCode) < requestedSeats)
            {
                hasEnoughSeatsEverywhere = false;
            }
        }

        if (!hasEnoughSeatsEverywhere || !_repository.TrySaveBatch(requests))
        {
            return false;
        }

        foreach (var request in requests)
        {
            _sender.Send(request);
        }

        return true;
    }
}
