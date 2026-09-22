namespace LkpServer.Domain;

internal sealed record CarLocation(
    double Latitude,
    double Longitude,
    DateTime TimestampUtc);
