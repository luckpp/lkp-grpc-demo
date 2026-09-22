using Dummy;
using Grpc.Core;

namespace LkpClient;

class Program
{
    const string Target = "localhost:50051";

    static void Main(string[] args)
    {
        Channel channel = new Channel(Target, ChannelCredentials.Insecure);

        channel.ConnectAsync().ContinueWith(task =>
        {
            if (task.Status == TaskStatus.RanToCompletion)
            {
                Console.WriteLine("Connected to the server.");
            }
            else
            {
                Console.WriteLine($"Failed to connect to the server: {task.Exception?.Message}");
            }
        }).Wait();

        var client = new DummyService.DummyServiceClient(channel);

        channel.ShutdownAsync().Wait();

        Console.ReadKey();
    }
}
