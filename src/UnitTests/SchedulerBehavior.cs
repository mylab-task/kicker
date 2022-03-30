using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using MyLab.Task.Scheduler;
using Quartz;
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
            var intervalConfigurator = new TestIntervalTriggerConfigurator();
            
            var kickService = new TestTaskKickerService();

            var actualJob = new JobOptions();
            var jobsConfig = new JobOptionsConfig
            {
                Jobs = new []{ actualJob }
            };

            var host = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddLogging(l => l
                        .AddFilter(f => true)
                        .AddXUnit(_output)
                    );

                    services.AddSingleton<ITaskKickerService, TestTaskKickerService>();
                    services.AddSchedulerLogic(jobsConfig, intervalConfigurator);
                })
                .Build();

            //Act
            var runTask = host.RunAsync();

            DateTime startTime = DateTime.Now;
            
            await Task.Delay(TimeSpan.FromSeconds(2));

            await host.StopAsync();

            //Assert
            Assert.NotNull(kickService);
            Assert.True(kickService.LastKickDt > startTime.AddSeconds(1));
            Assert.True(kickService.LastKickDt < startTime.AddSeconds(2));
            Assert.Equal(actualJob.Headers, kickService.LastKickOptions.Headers);
            Assert.Equal(actualJob.Host, kickService.LastKickOptions.Host);
            Assert.Equal(actualJob.Path, kickService.LastKickOptions.Path);
            Assert.Equal(actualJob.Port, kickService.LastKickOptions.Port);
        }

        class TestIntervalTriggerConfigurator : IIntervalTriggerConfigurator
        {
            public ITriggerConfigurator Configure(ITriggerConfigurator triggerConfigurator, JobOptions jobOptions)
            {
                return triggerConfigurator.WithSimpleSchedule(a => a.WithIntervalInSeconds(1));
            }
        }

        class TestTaskKickerService : ITaskKickerService
        {
            public KickOptions LastKickOptions { get; private set; }
            public DateTime LastKickDt { get; private set; }

            public Task KickAsync(KickOptions kickOptions)
            {
                LastKickOptions = LastKickOptions;
                LastKickDt = DateTime.Now;

                return Task.CompletedTask;
            }
        }
    }
}
