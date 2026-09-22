using EvDemo;
using Grpc.Core;
using LkpServer.Services;

namespace LkpServer.Grpc;

internal sealed class EvCarGrpcService(
    EvCarRepository carRepository,
    CarLocationSimulator locationSimulator) : EvCarService.EvCarServiceBase
{
    public override Task<CarStatusResponse> GetCarStatus(CarRequest request, ServerCallContext context)
    {
        var status = carRepository.GetStatus(request.CarId);

        if (status is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Car '{request.CarId}' was not found."));
        }

        return Task.FromResult(new CarStatusResponse
        {
            CarId = status.CarId,
            BatteryPercentage = status.BatteryPercentage,
            ChargingStatus = status.ChargingStatus,
            EstimatedRemainingRangeKm = status.EstimatedRemainingRangeKm
        });
    }

    public override async Task GetCarLocationRealTime(
        CarRequest request,
        IServerStreamWriter<CarLocationUpdate> responseStream,
        ServerCallContext context)
    {
        var status = carRepository.GetStatus(request.CarId);

        if (status is null || !locationSimulator.HasRoute(request.CarId))
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Car '{request.CarId}' was not found."));
        }

        // Server streaming keeps one request open while the server sends multiple updates.
        for (var sequenceNumber = 0; sequenceNumber < 8; sequenceNumber++)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                break;
            }

            var location = locationSimulator.CreateUpdate(request.CarId, sequenceNumber);

            await responseStream.WriteAsync(new CarLocationUpdate
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                TimestampUtc = location.TimestampUtc.ToString("O")
            });

            if (sequenceNumber == 7)
            {
                continue;
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
