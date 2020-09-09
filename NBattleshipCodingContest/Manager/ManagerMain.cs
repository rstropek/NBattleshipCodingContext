namespace NBattleshipCodingContest.Manager
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Microsoft.AspNetCore.Builder;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    internal class ManagerMain
    {
        [SuppressMessage("Microsoft.Performance", "CA1822", Scope = "method", Justification = "Static method could not act as context for logging")]
        public async Task StartManager(ManagerOptions options)
        {
            var logger = Log.Logger.ForContext<ManagerMain>();

            await Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureServices(services =>
                        {
                            services.AddGrpc();
                        })
                        .Configure((context, app) =>
                        {
                            if (context.HostingEnvironment.IsDevelopment())
                            {
                                app.UseDeveloperExceptionPage();
                            }

                            app.UseRouting();
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapGrpcService<ManagerService>();
                            });
                        })
                        .UseUrls(options.ManagerUrl);
                })
                .UseSerilog()
                .UseConsoleLifetime()
                .Build()
                .RunAsync();
        }
    }
}
