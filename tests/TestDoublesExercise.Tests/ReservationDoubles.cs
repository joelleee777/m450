
using TestDoublesExercise;

namespace TestDoublesExercise.Tests;

public class ReservationRepositoryStub : IReservationRepository
{
    public bool TrySave(Reservation reservation)
    {
        return true;
    }
}

public class ConfirmationSenderStub : IConfirmationSender
{
    public void Send(Reservation reservation)
    {
    }
}
