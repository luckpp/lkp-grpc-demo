using Grpc.Core;

namespace LkpServer;

class Program
{
    const int Port = 50051;

    static void Main(string[] args)
    {
        Server server = null;

        try
        {
            server = new Server
            {
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };

            server.Start();
            Console.WriteLine("The server is listening on port " + Port);
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
