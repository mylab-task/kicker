using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace MyLab.Task.Scheduler
{
    static class SchedulerLogicIntegration
    {
        public static void AddSchedulerLogic(this IServiceCollection services, JobOptionsConfig jobsConfig, IIntervalTriggerConfigurator intervalConfigurator)
        {
            services.AddHttpClient();

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                if (jobsConfig.Jobs != null)
                {
                    foreach (var jobOptions in jobsConfig.Jobs)
                    {
                        RegisterTaskKickJob(q, intervalConfigurator, jobOptions);
                    }
                }
            });

            services.AddQuartzHostedService();
        }

        static void RegisterTaskKickJob(IServiceCollectionQuartzConfigurator configurator, IIntervalTriggerConfigurator intervalConfigurator, JobOptions jobOptions)
        {
            var jobKey = new JobKey(jobOptions.Id);

            configurator
                .AddJob<KickTaskJob>(c => c
                    .WithIdentity(jobKey)
                    .UsingJobData(jobOptions.ToJobDataMap())
                )
                .AddTrigger(c =>
                {
                    c
                        .ForJob(jobKey)
                        .WithIdentity(jobKey + "-trigger");

                    intervalConfigurator.Configure(c, jobOptions);
                });

        }
    }
}