using System.Net;

namespace MyLab.Task.Scheduler
{
    public class TaskKickResult
    {
        public string Response { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}