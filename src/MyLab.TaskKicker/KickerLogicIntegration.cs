using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace MyLab.TaskKicker
{
    static class KickerLogicIntegration
    {
        public static void AddKickerLogic(this IServiceCollection services, JobOptionsConfig jobsConfig)
        {
            services.AddHttpClient();

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                if (jobsConfig.Jobs != null)
                {
                    foreach (var jobOptions in jobsConfig.Jobs)
                    {
                        RegisterTaskKickJob(q, jobOptions);
                    }
                }
            });

            services.AddQuartzHostedService();
        }

        static void RegisterTaskKickJob(IServiceCollectionQuartzConfigurator configurator, JobOptions jobOptions)
        {
            var jobKey = new JobKey(jobOptions.Id);

            configurator
                .AddJob<KickTaskJob>(c => c
                    .WithIdentity(jobKey)
                    .UsingJobData(jobOptions.ToJobDataMap())
                )
                .AddTrigger(c => c
                        .ForJob(jobKey)
                        .WithIdentity(jobKey + "-trigger")
                        .WithCronSchedule(jobOptions.Cron)
                );

        }
    }
}