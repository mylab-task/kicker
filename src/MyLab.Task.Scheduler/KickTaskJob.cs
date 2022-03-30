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
        private readonly HttpClient _httpClient;
        private readonly IDslLogger _logger;

        public KickTaskJob(HttpClient httpClient, ILogger<KickTaskJob> logger)
        {
            _httpClient = httpClient;
            _logger = logger.Dsl();
        }

        public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            var opts = JobOptions.FromJobDataMap(context.JobDetail.JobDataMap);

            try
            {
                var uriB = new UriBuilder("http", opts.Host, opts.Port, opts.Path);

                using var request = new HttpRequestMessage(HttpMethod.Post, uriB.Uri);

                if (opts.Headers != null)
                {
                    foreach (var optsHeader in opts.Headers)
                    {
                        request.Headers.Add(optsHeader.Key, optsHeader.Value);
                    }
                }

                var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

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
