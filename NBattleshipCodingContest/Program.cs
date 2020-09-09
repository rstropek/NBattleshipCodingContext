namespace NBattleshipCodingContest
{
    using CommandLine;
    using Microsoft.Extensions.Configuration;
    using NBattleshipCodingContest.Manager;
    using NBattleshipCodingContest.Players;
    using Serilog;
    using System;
    using System.IO;

    class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
             .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
             .AddEnvironmentVariables()
             .Build();

        static void Main(string[] args)
        {
            // Setup logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("App Name", "NBattleshipCodingContest")
                .CreateLogger();

            // Parse command line arguments and start services
            // depending on given verb.
            Parser.Default.ParseArguments<ManagerOptions, BattleHostOptions>(args)
                .MapResult(
                  (ManagerOptions options) => StartRunnerAndReturnExitCode(options),
                  (BattleHostOptions options) => StartBattleHostAndReturnExitCode(options),
                  errors => 1);
        }

        private static int StartBattleHostAndReturnExitCode(BattleHostOptions options)
        {
            var log = Log.Logger.ForContext<Program>();
            log.Information("Starting battle host process...");
            var host = new BattleHostMain();
            host.StartBattleHost(options).Wait();
            log.Information("Battle host process ended.");
            return 0;
        }

        private static int StartRunnerAndReturnExitCode(ManagerOptions options)
        {
            var log = Log.Logger.ForContext<Program>();
            log.Information("Starting manager process...");
            var manager = new ManagerMain();
            manager.StartManager(options).Wait();
            log.Information("Manager process ended.");
            return 0;
        }
    }
}
