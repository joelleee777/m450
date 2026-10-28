
using TestDoublesExercise;

namespace TestDoublesExercise.Tests;

public class ReservationRepositoryStub : IReservationRepository
{
    private readonly bool _saveResult;

    public ReservationRepositoryStub(bool saveResult = true)
    {
        _saveResult = saveResult;
    }

    public bool TrySave(Reservation reservation)
    {
        return _saveResult;
    }
}

public class ConfirmationSenderStub : IConfirmationSender
{
    public void Send(Reservation reservation)
    {
    }
}
