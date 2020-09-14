namespace NBattleshipCodingContest.Manager
{
    using Grpc.Core;

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


    }
}
