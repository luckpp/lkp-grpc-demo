using LkpServer.Domain;

namespace LkpServer.Services;

internal sealed class CarLocationSimulator
{
    private readonly Dictionary<string, (double Latitude, double Longitude)> _startingPoints = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ev1"] = (44.4268, 26.1025),
        ["ev2"] = (44.4378, 26.0979),
        ["ev3"] = (44.4432, 26.1039)
    };

    public bool HasRoute(string carId)
    {
        return _startingPoints.ContainsKey(carId);
    }

    public CarLocation CreateUpdate(string carId, int sequenceNumber)
    {
        var start = _startingPoints[carId];
        var latitude = start.Latitude + (sequenceNumber * 0.0012);
        var longitude = start.Longitude + (sequenceNumber * 0.0009);

        return new CarLocation(latitude, longitude, DateTime.UtcNow);
    }
}
