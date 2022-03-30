using System;
using System.Net.Http;
using System.Text;

namespace MyLab.Task.Scheduler
{
    class TaskKickerService : ITaskKickerService
    {
        private readonly HttpClient _httpClient;

        public TaskKickerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async System.Threading.Tasks.Task<TaskKickResult> KickAsync(KickOptions opts)
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

            await using var readStream = await response.Content.ReadAsStreamAsync();

            byte[] respContentBuff = new byte[2000];
            var read = await readStream.ReadAsync(respContentBuff, 0, respContentBuff.Length);

            return new TaskKickResult
            {
                Response = Encoding.UTF8.GetString(respContentBuff, 0, read),
                StatusCode = response.StatusCode
            };
        }
    }
}