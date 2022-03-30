using System;
using System.Collections.Generic;
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
    public partial class SchedulerBehavior
    {
        [Fact]
        public async Task ShouldKickTask()
        {
            //Arrange
            var actualJob = new JobOptions
            {
                Cron = "0/1 * * * * ?"
            };
            var kickService = new TestTaskKickerService();

            //Act
            await KickTaskAsync(actualJob, kickService);

            //Assert
            Assert.Equal(2, kickService.KickCount);
        }

        [Fact]
        public async Task ShouldKickTaskWithSpecifiedParameters()
        {
            //Arrange
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
            var kickService = new TestTaskKickerService();
            
            //Act
            await KickTaskAsync(actualJob, kickService);

            //Assert
            Assert.Equal(actualJob.Host, kickService.LastKickOptions.Host);
            Assert.Equal(actualJob.Path, kickService.LastKickOptions.Path);
            Assert.Equal(actualJob.Port, kickService.LastKickOptions.Port);
            Assert.Equal(actualJob.Headers, kickService.LastKickOptions.Headers);
        }

        [Fact]
        public async Task ShouldKickTaskWithDefaultParameters()
        {
            //Arrange
            var actualJob = new JobOptions
            {
                Cron = "0/1 * * * * ?",
            };
            var kickService = new TestTaskKickerService();

            //Act
            await KickTaskAsync(actualJob, kickService);

            //Assert
            Assert.Null(kickService.LastKickOptions.Host);
            Assert.Equal(JobOptions.DefaultPath, kickService.LastKickOptions.Path);
            Assert.Equal(JobOptions.DefaultPort, kickService.LastKickOptions.Port);
            Assert.Null(kickService.LastKickOptions.Headers);
        }
    }
}
