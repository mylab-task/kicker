using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyLab.Task.Scheduler;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class SchedulerBehavior
    {
        private readonly ITestOutputHelper _output;

        public SchedulerBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task ShouldKickTask()
        {
            //Arrange
            var kickService = new TestTaskKickerService();

            var actualJob = new JobOptions
            {
                Cron = "0/1 * * * * ?",
                Host = "foo",
                Path = "/bar",
                Port = 8080,
                Headers = new Dictionary<string, string>
                {
                    {"foo", "bar"}
                }
            };

            //Act
            var jobsConfig = new JobOptionsConfig
            {
                Jobs = new[] { actualJob }
            };

            using var host = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddLogging(l => l
                        .AddFilter(f => true)
                        .AddXUnit(_output)
                    );

                    services.AddSingleton<ITaskKickerService>(kickService);
                    services.AddSchedulerLogic(jobsConfig);
                })
                .Build();

            await host.StartAsync();

            await Task.Delay(TimeSpan.FromSeconds(0.9));

            await host.StopAsync();

            //Assert
            Assert.Equal(2, kickService.KickCount);

            Assert.Equal(actualJob.Host, kickService.LastKickOptions.Host);
            Assert.Equal(actualJob.Path, kickService.LastKickOptions.Path);
            Assert.Equal(actualJob.Port, kickService.LastKickOptions.Port);
            Assert.Equal(actualJob.Headers, kickService.LastKickOptions.Headers);
        }

        private class TestTaskKickerService : ITaskKickerService
        {
            public int KickCount { get; private set; }

            public KickOptions LastKickOptions { get; private set; }

            public Task<TaskKickResult> KickAsync(KickOptions kickOptions)
            {
                KickCount += 1;
                LastKickOptions = kickOptions;

                return Task.FromResult(new TaskKickResult());
            }
        }
    }
}
