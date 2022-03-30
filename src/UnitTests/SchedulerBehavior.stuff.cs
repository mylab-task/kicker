using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyLab.Task.Scheduler;
using Xunit.Abstractions;

namespace UnitTests
{
    public partial class SchedulerBehavior
    {
        private readonly ITestOutputHelper _output;

        public SchedulerBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        private async Task KickTaskAsync(JobOptions jobOptions, ITaskKickerService kicker)
        {
            var jobsConfig = new JobOptionsConfig
            {
                Jobs = new[] { jobOptions }
            };

            var host = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddLogging(l => l
                        .AddFilter(f => true)
                        .AddXUnit(_output)
                    );

                    services.AddSingleton<ITaskKickerService>(kicker);
                    services.AddSchedulerLogic(jobsConfig);
                })
                .Build();
            
            await host.StartAsync();

            await Task.Delay(TimeSpan.FromSeconds(0.9));

            await host.StopAsync();
        }

        private class TestTaskKickerService : ITaskKickerService
        {
            public int KickCount { get; private set; }

            public KickOptions LastKickOptions { get; private set; }

            public Task KickAsync(KickOptions kickOptions)
            {
                KickCount += 1;
                LastKickOptions = kickOptions;

                return Task.CompletedTask;
            }
        }
    }
}