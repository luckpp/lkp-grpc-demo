namespace LkpServer.Domain;

internal sealed record EvCarStatus(
    string CarId,
    int BatteryPercentage,
    string ChargingStatus,
    int EstimatedRemainingRangeKm);
