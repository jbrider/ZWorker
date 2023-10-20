
using NetMQ;
using NetMQ.Sockets;

namespace ZWorker;

public class WorkerService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(() =>
        {
            //get address of jobs service
            //Start reading jobs
            Console.WriteLine("Listening for jobs...");

            using (var receiver = new PullSocket(">tcp://ZeroServer:5557"))
            using (var sender = new PushSocket(">tcp://ZeroServer:5558"))
            {
                //process tasks forever
                while (true)
                {
                    //to simulate some work being done, see
                    //In real life some more meaningful work would be done
                    string workload = receiver.ReceiveFrameString();
                    Console.WriteLine($"workload received:{workload}");
                    //simulate some work being done
                    Thread.Sleep(int.Parse(workload));
                    //send results to sink, sink just needs to know worker
                    //is done, message content is not important, just the presence of
                    //a message means worker is done.
                    Console.WriteLine("Sending to Sink");
                    sender.SendFrame(string.Empty);
                }
            }
        });
    }
}
