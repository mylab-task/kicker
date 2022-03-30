using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using MyLab.Log.Dsl;
using Quartz;

namespace MyLab.Task.Scheduler
{
    [DisallowConcurrentExecution]
    public class KickTaskJob : IJob
    {
        private readonly ITaskKickerService _taskKickerService;
        private readonly IDslLogger _logger;

        public KickTaskJob(ITaskKickerService taskKickerService, ILogger<KickTaskJob> logger)
        {
            _taskKickerService = taskKickerService;
            _logger = logger.Dsl();
        }

        public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            var opts = JobOptions.FromJobDataMap(context.JobDetail.JobDataMap);

            try
            {
                var kickOptions = new KickOptions(opts);

                await _taskKickerService.KickAsync(kickOptions);

                _logger.Action("Task kicked successfully")
                    .AndFactIs("job-id", opts.Id)
                    .Write();
            }
            catch (Exception e)
            {
                _logger.Error(e)
                    .AndFactIs("job-id", opts.Id)
                    .Write();
            }
        }
    }
}
