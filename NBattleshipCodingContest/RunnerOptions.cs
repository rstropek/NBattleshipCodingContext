namespace NBattleshipCodingContest
{
    using CommandLine;

    [Verb("runner", HelpText = "Starts a battleship game runner process (gRPC server).")]
    public class RunnerOptions
    {
        [Option("port", Default = (ushort)5001)]
        public ushort UshortValue { get; set; }
    }
}
