using System.Net;

namespace MyLab.TaskKicker
{
    public class TaskKickResult
    {
        public string Response { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}