namespace NBattleshipCodingContest.Manager
{
    using Grpc.Core;
    using Grpc.Core.Logging;
    using Microsoft.Extensions.Logging;
    using NBattleshipCodingContest.Logic;
    using NBattleshipCodingContest.Players;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class BattleManagerConnection
    {
        public BattleManagerConnection(ILogger<BattleHostService> logger)
        {
            this.logger = logger;
        }

        private IClientStreamWriter<Status>? requestStream;
        public IClientStreamWriter<Status>? RequestStream
        {
            get => requestStream;

            set
            {
                requestStream = value;
            }
        }

        private TaskCompletionSource<Logic.SquareContent>? shotCompletion;
        private readonly ILogger<BattleHostService> logger;

        public void GetShoot(RequestShot request)
        {
            logger.LogInformation("Processing get shot request");

            shotCompletion = new TaskCompletionSource<Logic.SquareContent>();

            var shooter = PlayerList.Players[request.Shooter];
            var board = new BoardContent(request.Board);
            shooter.GetShot(board, loc =>
            {
                logger.LogInformation($"Sending shot location {loc} to battle manager");

                if (RequestStream == null)
                {
                    throw new InvalidOperationException("No request stream. Should never happen!");
                }

                RequestStream.WriteAsync(new Status
                {
                    Shot = new Shot { Location = loc }
                }).ContinueWith(t =>
                {
                    if (t.IsFaulted && t.Exception != null)
                    {
                        logger.LogError(t.Exception, "Error while talking to battle manager");
                        shotCompletion.TrySetException(t.Exception);
                    }
                });

                return shotCompletion.Task;
            });
        }

        public void ProcessShotResult(ShotResult result)
        {
            logger.LogInformation("Processing shot result");

            if (shotCompletion == null)
            {
                throw new InvalidOperationException("Wrong internal state, should never happen!");
            }

            shotCompletion.TrySetResult((Logic.SquareContent)result.SquareContent);
        }
    }
}
