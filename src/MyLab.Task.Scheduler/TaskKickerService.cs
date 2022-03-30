using System;
using System.Net.Http;

namespace MyLab.Task.Scheduler
{
    class TaskKickerService : ITaskKickerService
    {
        private readonly HttpClient _httpClient;

        public TaskKickerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async System.Threading.Tasks.Task KickAsync(KickOptions opts)
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
        }
    }
}