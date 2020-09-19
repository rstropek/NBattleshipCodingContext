namespace NBattleshipCodingContest.Players
{
    using Grpc.Core;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NBattleshipCodingContest.Logic;
    using NBattleshipCodingContest.Manager;
    using System.Threading;
    using System.Threading.Tasks;
    using static NBattleshipCodingContest.Manager.Manager;

    internal class BattleHostService : IHostedService
    {
        private readonly ILogger<BattleHostService> logger;
        private readonly ManagerClient client;
        private readonly IHostApplicationLifetime appLifetime;
        private AsyncDuplexStreamingCall<NBattleshipCodingContest.Manager.Status, GameRequest>? Connection;

        public BattleHostService(ILogger<BattleHostService> logger, ManagerClient client,
            IHostApplicationLifetime appLifetime)
        {
            this.logger = logger;
            this.client = client;
            this.appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Register callback for application stopping event. App shutdown
            // will be delayed until this callback is finished.
            appLifetime.ApplicationStopping.Register(() =>
            {
                if (Connection != null)
                {
                    logger.LogInformation("Stopping battle host.");
                    Connection.RequestStream.CompleteAsync().Wait();
                }
            });

            Connection = client.Connect(cancellationToken: cancellationToken);

            // Start background task handling incoming battle requests.
            Task.Run(async () =>
            {
                var managerConnection = new BattleManagerConnection(logger)
                {
                    RequestStream = Connection.RequestStream
                };

                try
                { 
                    await foreach (var item in Connection.ResponseStream.ReadAllAsync(CancellationToken.None))
                    {
                        switch (item.PayloadCase)
                        {
                            case GameRequest.PayloadOneofCase.RequestShot:
                                managerConnection.GetShoot(item.RequestShot);
                                break;
                            case GameRequest.PayloadOneofCase.ShotResult:
                                managerConnection.ProcessShotResult(item.ShotResult);
                                break;
                            default:
                                logger.LogInformation("Received unknown payload type {PayloadCase}", item.PayloadCase);
                                break;
                        }
                    }
                }
                finally
                {
                    // When connection to manager stops, the application should stop.
                    appLifetime.StopApplication();
                }
            }, CancellationToken.None);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken _) => Task.CompletedTask;
    }
}
