using System.Linq;
using MyLab.TaskKicker;
using Xunit;

namespace UnitTests
{
    public class JobOptionsConfigBehavior
    {
        private readonly JobOptionsConfig _config;

        public JobOptionsConfigBehavior()
        {
            _config = JobOptionsConfig.Load("config.yml");
        }

        [Fact]
        public void ShouldDeserializeYamlFile()
        {
            //Arrange
            

            //Act
            var fooJob = _config?.Jobs?.FirstOrDefault(j => j.Id == "foo");

            //Assert
            Assert.NotNull(_config);
            Assert.NotNull(fooJob);
        }

        [Fact]
        public void ShouldDeserializeJobOptions()
        {
            //Arrange


            //Act
            var fooJob = _config.Jobs.First(j => j.Id == "foo");

            //Assert
            Assert.Equal("localhost", fooJob.Host);
            Assert.Equal("* * * *", fooJob.Cron);
            Assert.Equal("/path", fooJob.Path);
            Assert.Equal(8080, fooJob.Port);
            Assert.NotNull(fooJob.Headers);
            Assert.True(fooJob.Headers.ContainsKey("foo-key"));
            Assert.Equal("foo-val", fooJob.Headers["foo-key"]);
            Assert.True(fooJob.Headers.ContainsKey("bar-key"));
            Assert.Equal("bar-val", fooJob.Headers["bar-key"]);
        }

        [Fact]
        public void ShouldUseDefaultsInJobOptions()
        {
            //Arrange


            //Act
            var fooJob = _config.Jobs.First(j => j.Id == "bar");

            //Assert
            Assert.Null(fooJob.Host);
            Assert.Equal(JobOptions.DefaultCron, fooJob.Cron);
            Assert.Equal(JobOptions.DefaultPath, fooJob.Path);
            Assert.Equal(JobOptions.DefaultPort, fooJob.Port);
            Assert.Null(fooJob.Headers);
        }
    }
}
