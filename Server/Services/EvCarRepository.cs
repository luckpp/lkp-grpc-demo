using LkpServer.Domain;

namespace LkpServer.Services;

internal sealed class EvCarRepository
{
    private readonly Dictionary<string, EvCarStatus> _cars = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ev1"] = new("ev1", 82, "Driving", 320),
        ["ev2"] = new("ev2", 58, "Charging", 210),
        ["ev3"] = new("ev3", 34, "Parked", 140)
    };

    public EvCarStatus? GetStatus(string carId)
    {
        return _cars.TryGetValue(carId, out var status) ? status : null;
    }

    public IReadOnlyCollection<string> GetAvailableCarIds()
    {
        return _cars.Keys.ToArray();
    }
}
