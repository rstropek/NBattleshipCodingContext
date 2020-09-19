namespace NBattleshipCodingContest.Manager
{
    using Google.Protobuf;
    using Grpc.Core;
    using Microsoft.Extensions.Logging;
    using NBattleshipCodingContest.Logic;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    internal enum BattleHostState
    {
        NotConnected,
        Idle,
        WaitingForShot
    }

    public class BattleHostConnection
    {
        public BattleHostConnection(ILogger<BattleHostConnection> logger)
        {
            this.logger = logger;
        }

        private IServerStreamWriter<GameRequest>? responseStream;
        public IServerStreamWriter<GameRequest>? ResponseStream
        {
            get => responseStream;

            set
            {
                responseStream = value;
            }
        }

        public bool IsHostConnected => responseStream != null;

        private TaskCompletionSource? shootCompletion;
        private readonly ILogger<BattleHostConnection> logger;
        private BoardContent? shots;
        private IReadOnlyBoard? board;
        private UUID? gameUUID;

        protected virtual Task Delay() => Task.Delay(TimeSpan.FromMinutes(20));

        public Task Shoot(Guid gameId, int shooter, int opponent, BoardContent shots, IReadOnlyBoard board)
        {
            logger.LogInformation("Entering shooting process");

            shootCompletion = new TaskCompletionSource();
            this.shots = shots;
            this.board = board;
            gameUUID = new UUID { Value = gameId.ToString() };

            // Setup timeout task
            Delay().ContinueWith(t =>
                {
                    logger.LogWarning("Timeout, exiting shooting process");
                    shootCompletion.TrySetCanceled();
                });

            // Send shoot
            SendShoot(shooter, opponent, shots)
                .ContinueWith(t =>
                {
                    // Error during shot
                    if (t.IsFaulted && t.Exception != null)
                    {
                        logger.LogError(t.Exception, "Error while talking to battle host");
                        shootCompletion.TrySetException(t.Exception);
                    }
                });

            return shootCompletion.Task;
        }

        private async Task SendShoot(int shooter, int opponent, IReadOnlyBoard board)
        {
            logger.LogInformation("Sending shot request to battle host");
            await ResponseStream!.WriteAsync(new GameRequest
            {
                RequestShot = new RequestShot
                {
                    Shooter = shooter,
                    Opponent = opponent,
                    GameId = gameUUID,
                    Board = ByteString.CopyFrom(board.Cast<byte>().ToArray())
                }
            });
        }

        private async Task SendShotResult(Logic.SquareContent content)
        {
            logger.LogInformation("Sending shot result to battle host");
            await ResponseStream!.WriteAsync(new GameRequest
            {
                ShotResult = new ShotResult
                {
                    GameId = gameUUID,
                    SquareContent = (SquareContent)content
                }
            });
        }

        public async Task ProcessShot(Shot shot)
        {
            logger.LogInformation($"Processing incoming shot to {shot.Location}");

            if (shootCompletion == null || shots == null || board == null)
            {
                throw new InvalidOperationException("Wrong internal state, should never happen!");
            }

            if (BoardIndex.TryParse(shot.Location, out var ix))
            {
                var content = board[ix];
                if (content == Logic.SquareContent.Ship)
                {
                    content = Logic.SquareContent.HitShip;
                }

                logger.LogInformation($"Sending shot result to battle host (content = {content})");
                await SendShotResult(content);
                shots[ix] = content;

                logger.LogInformation("Shooting process successfully completed");
                shootCompletion.TrySetResult();
                return;
            }

            shootCompletion.TrySetException(new Exception("Illegal shot"));
        }
    }
}
