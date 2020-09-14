namespace NBattleshipCodingContest.Manager
{
    using Grpc.Core;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using static NBattleshipCodingContest.Manager.Manager;

    internal class ManagerService : ManagerBase
    {
        private static bool BattleRunnerConnected;
        private static readonly object BattleRunnerConnectedLockObject = new object();
        private readonly ILogger<ManagerService> logger;
        private readonly IHostApplicationLifetime appLifetime;
        private readonly BattleHostConnection battleHostConnection;

        public ManagerService(ILogger<ManagerService> logger, IHostApplicationLifetime appLifetime, BattleHostConnection battleHostConnection)
        {
            this.logger = logger;
            this.appLifetime = appLifetime;
            this.battleHostConnection = battleHostConnection;
        }

        public override async Task Connect(IAsyncStreamReader<Status> requestStream, IServerStreamWriter<GameRequest> responseStream, ServerCallContext context)
        {
            // Make sure that only one battle host is running
            lock (BattleRunnerConnectedLockObject)
            {
                if (BattleRunnerConnected)
                {
                    logger.LogWarning("Another battle runner tried to connect, declining.");
                    context.Status = new Grpc.Core.Status(StatusCode.ResourceExhausted,
                        "A battle runner is already connected. Currently, multiple battle runners are not supported.");
                    return;
                }

                BattleRunnerConnected = true;
                battleHostConnection.ResponseStream = responseStream;
            }

            // Cancellation token source used to cancel the gRPC call
            var cts = new CancellationTokenSource();
            using var registration = appLifetime.ApplicationStopping.Register(() =>
            {
                // If manager shuts down, we cancel the gRPC call
                cts.Cancel();

                // Give it a second to process the cancellation
                Task.Delay(100).Wait();
            });

            try
            {
                logger.LogInformation("New battle runner connected.");
                await foreach (var item in requestStream.ReadAllAsync(cts.Token))
                {
                    switch (item.PayloadCase)
                    {
                        case Status.PayloadOneofCase.Shot:
                            logger.LogInformation("Received shot to {Location} for game {GameID}.", item.Shot.GameId, item.Shot.Location);
                            break;
                        case Status.PayloadOneofCase.Crash:
                            logger.LogWarning("Received crash of game {GameID}.", item.Crash.GameId);
                            break;
                        default:
                            logger.LogWarning("Received unknown payload type {PayloadCase}", item.PayloadCase);
                            break;
                    }
                }

                logger.LogInformation("Battle runner exited");
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Manager is shutting down");
            }
            finally
            {
                lock (BattleRunnerConnectedLockObject)
                {
                    BattleRunnerConnected = false;
                    battleHostConnection.ResponseStream = null;
                }
            }
        }
    }
}
