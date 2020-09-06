using CommandLine;
using System;

namespace NBattleshipCodingContest
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<RunnerOptions>(args)
                .MapResult(
                  (RunnerOptions options) => StartRunnerAndReturnExitCode(options),
                  errors => 1);
        }

        static int StartRunnerAndReturnExitCode(RunnerOptions options)
        {
            return 0;
        }
    }
}
