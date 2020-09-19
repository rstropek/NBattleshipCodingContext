namespace NBattleshipCodingContest.Manager
{
    using Google.Protobuf;
    using Grpc.Core;
    using NBattleshipCodingContest.Logic;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal enum BattleHostState
    {
        NotConnected,
        Idle,
        WaitingForShot
    }

    internal class BattleHostConnection
    {
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

        private TaskCompletionSource<string>? shootCompletion;
        private readonly object shootCompletionLockObject = new object();
        private BoardContent? shots;
        private IReadOnlyBoard? board;
        private UUID gameUUID;

        protected virtual Task Delay() => Task.Delay(250);

        public Task<string> Shoot(Guid gameId, int shooter, int opponent, BoardContent shots, IReadOnlyBoard board)
        {
            if (!Monitor.TryEnter(shootCompletionLockObject))
            {
                throw new InvalidOperationException("Another shoot is still in progress");
            }

            shootCompletion = new TaskCompletionSource<string>();
            this.shots = shots;
            this.board = board;
            gameUUID = new UUID { Value = gameId.ToString() };

            // Setup timeout task
            Delay().ContinueWith(t =>
                {
                    shootCompletion.TrySetCanceled();
                    Monitor.Exit(shootCompletionLockObject);
                })
                .ConfigureAwait(false);

            // Send shoot
            SendShoot(shooter, opponent, shots)
                .ContinueWith(t =>
                {
                    // Error during shot
                    if (t.IsFaulted && t.Exception != null)
                    {
                        shootCompletion.TrySetException(t.Exception);
                        Monitor.Exit(shootCompletionLockObject);
                    }
                })
                .ConfigureAwait(false);

            return shootCompletion.Task;
        }

        private async Task SendShoot(int shooter, int opponent, IReadOnlyBoard board)
        {
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
            await ResponseStream!.WriteAsync(new GameRequest
            {
                ShotResult = new ShotResult
                {
                    GameId = gameUUID,
                    SquareContent = content switch
                    {
                        Logic.SquareContent.Unknown => SquareContent.Unknown,
                        Logic.SquareContent.Water => SquareContent.Water,
                        Logic.SquareContent.Ship => SquareContent.Ship,
                        Logic.SquareContent.HitShip => SquareContent.HitShip,
                        _ => throw new ArgumentOutOfRangeException(nameof(content))
                    }
                }
            });
        }

        public async Task ProcessShot(Shot shot)
        {
            if (!Monitor.IsEntered(shootCompletionLockObject))
            {
                throw new InvalidOperationException("No shot in progress");
            }

            if (shootCompletion == null || shots == null || board == null)
            {
                throw new InvalidOperationException("Wrong internal state, should never happen!");
            }

            if (BoardIndex.TryParse(shot.Location, out var ix))
            {
                var content = board[ix];
                await SendShotResult(content);
                shots[ix] = board[ix];

                shootCompletion.TrySetResult(shot.Location);
                return;
            }

            shootCompletion.TrySetException(new Exception("Illegal shot"));
            Monitor.Exit(shootCompletionLockObject);
        }
    }
}
