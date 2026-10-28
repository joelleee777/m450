
using TestDoublesExercise;

namespace TestDoublesExercise.Tests;

public class SeatAvailabilityStub : ISeatAvailability
{
    private readonly int _availableSeats;

    public SeatAvailabilityStub(int availableSeats)
    {
        _availableSeats = availableSeats;
    }

    public int GetAvailableSeats(string eventCode)
    {
        return _availableSeats;
    }
}
