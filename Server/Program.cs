using EvDemo;
using Grpc.Core;
using LkpServer.Grpc;
using LkpServer.Services;

namespace LkpServer;

internal static class Program
{
    private const int Port = 50051;

    private static void Main(string[] args)
    {
        Server? server = null;

        try
        {
            var carRepository = new EvCarRepository();
            var locationSimulator = new CarLocationSimulator();

            server = new Server
            {
                Services =
                {
                    EvCarService.BindService(new EvCarGrpcService(carRepository, locationSimulator))
                },
                Ports =
                {
                    new ServerPort("localhost", Port, ServerCredentials.Insecure)
                }
            };

            server.Start();

            Console.WriteLine($"EV gRPC demo server is listening on port {Port}.");
            Console.WriteLine($"Try these car IDs: {string.Join(", ", carRepository.GetAvailableCarIds())}");
            Console.WriteLine("Press any key to stop the server.");
            Console.ReadKey();
        }
        catch (IOException e)
        {
            Console.WriteLine($"Error starting server: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unexpected error: {e.Message}");
        }
        finally
        {
            server?.ShutdownAsync().Wait();
        }
    }
}
