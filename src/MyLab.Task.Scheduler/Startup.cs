using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyLab.Log;
using Quartz;
using YamlDotNet.Serialization;

namespace MyLab.Task.Scheduler
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient();
            services.AddLogging(l => l.AddMyLabConsole());

            var jobsConfig = JobOptionsConfig.Load("jobs.yml");

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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
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
