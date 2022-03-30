using System.Collections.Generic;

namespace MyLab.Task.Scheduler
{
    public class KickOptions
    {
        public string Host { get; set; }
        public string Path { get; set; } 
        public int Port { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        
        public KickOptions()
        {
            
        }
        public KickOptions(JobOptions jobOptions)
        {
            Host = jobOptions.Host;
            Path = jobOptions.Path;
            Port = jobOptions.Port;
            if (jobOptions.Headers != null)
                Headers = new Dictionary<string, string>(jobOptions.Headers);
        }
    }
}