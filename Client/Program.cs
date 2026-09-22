using EvDemo;
using Grpc.Core;

namespace LkpClient;

internal static class Program
{
    private const string Target = "localhost:50051";

    private static async Task Main(string[] args)
    {
        var channel = new Channel(Target, ChannelCredentials.Insecure);

        try
        {
            await channel.ConnectAsync();
            Console.WriteLine("Connected to the EV demo server.");

            var client = new EvCarService.EvCarServiceClient(channel);
            await RunDemoAsync(client);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Unexpected client error: {exception.Message}");
        }
        finally
        {
            await channel.ShutdownAsync();
        }
    }

    private static async Task RunDemoAsync(EvCarService.EvCarServiceClient client)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Choose a demo action:");
            Console.WriteLine("1 - Get car status (Unary RPC)");
            Console.WriteLine("2 - Stream car location (Server Streaming RPC)");
            Console.WriteLine("0 - Exit");
            Console.Write("Selection: ");

            var selection = Console.ReadLine();
            if (selection == "0")
            {
                return;
            }

            var carId = PromptCarId();

            switch (selection)
            {
                case "1":
                    await ShowCarStatusAsync(client, carId);
                    break;
                case "2":
                    await ShowCarLocationStreamAsync(client, carId);
                    break;
                default:
                    Console.WriteLine("Unknown option. Try 1, 2, or 0.");
                    break;
            }
        }
    }

    private static string PromptCarId()
    {
        Console.Write("Car ID (ev1, ev2, ev3) [ev1]: ");
        var carId = Console.ReadLine();
        return string.IsNullOrWhiteSpace(carId) ? "ev1" : carId.Trim();
    }

    private static async Task ShowCarStatusAsync(EvCarService.EvCarServiceClient client, string carId)
    {
        try
        {
            var response = await client.GetCarStatusAsync(new CarRequest { CarId = carId });

            Console.WriteLine();
            Console.WriteLine("Current EV status:");
            Console.WriteLine($"Car ID: {response.CarId}");
            Console.WriteLine($"Battery: {response.BatteryPercentage}%");
            Console.WriteLine($"Charging status: {response.ChargingStatus}");
            Console.WriteLine($"Estimated range: {response.EstimatedRemainingRangeKm} km");
        }
        catch (RpcException exception)
        {
            Console.WriteLine($"gRPC error: {exception.Status.Detail}");
        }
    }

    private static async Task ShowCarLocationStreamAsync(EvCarService.EvCarServiceClient client, string carId)
    {
        try
        {
            using var call = client.GetCarLocationRealTime(new CarRequest { CarId = carId });

            Console.WriteLine();
            Console.WriteLine("Streaming live location updates:");

            while (await call.ResponseStream.MoveNext())
            {
                var update = call.ResponseStream.Current;
                Console.WriteLine($"{update.TimestampUtc} | Lat: {update.Latitude:F4}, Lng: {update.Longitude:F4}");
            }
        }
        catch (RpcException exception)
        {
            Console.WriteLine($"gRPC error: {exception.Status.Detail}");
        }
    }
}
