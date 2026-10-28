
using System.Collections.Generic;
using TestDoublesExercise;

namespace TestDoublesExercise.Tests;

public class ConfirmationSpy : IConfirmationSender
{
    public List<Reservation> SentReservations { get; } = new();

    public void Send(Reservation reservation)
    {
        SentReservations.Add(reservation);
    }
}
