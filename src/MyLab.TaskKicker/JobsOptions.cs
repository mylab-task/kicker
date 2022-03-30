using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using MyLab.Log;
using Quartz;
using YamlDotNet.Serialization;

namespace MyLab.TaskKicker
{
    class JobOptionsConfig
    {
        [YamlMember(Alias = "jobs")]
        public JobOptions[] Jobs { get; set; }

        public void CopyTo(JobOptionsConfig target)
        {
            target.Jobs = Jobs;
        }

        public static JobOptionsConfig Load(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("Job configuration file not found")
                    .AndFactIs("filename", filename);

            var jobsFileContent = File.ReadAllText(filename);

            var serializer = new DeserializerBuilder()
                .Build();

            return serializer.Deserialize<JobOptionsConfig>(jobsFileContent);
        }
    }

    public class JobOptions
    {
        private const string IdKey = "id";
        private const string CronKey = "cron";
        private const string HostKey = "host";
        private const string PathKey = "path";
        private const string PortKey = "port";
        private const string HeadersKey = "headers";

        public const string DefaultCron = "0 * * * *";
        public const string DefaultPath = "/processing";
        public const int DefaultPort = 80;

        [YamlMember(Alias = IdKey)]
        public string  Id { get; set; } = Guid.NewGuid().ToString("N");

        [YamlMember(Alias = CronKey)]
        public string Cron { get; set; } = DefaultCron;
        [YamlMember(Alias = HostKey)]
        public string Host { get; set; }
        [YamlMember(Alias = PathKey)] 
        public string Path { get; set; } = DefaultPath;
        [YamlMember(Alias = PortKey)]
        public int Port { get; set; } = DefaultPort;
        [YamlMember(Alias = HeadersKey)]
        public Dictionary<string,string> Headers { get; set; }

        public JobDataMap ToJobDataMap()
        {
            var map = new JobDataMap
            {
                { IdKey, Id },
                { CronKey, Cron },
                { HostKey, Host },
                { PathKey, Path },
                { PortKey, Port }
            };

            if (Headers != null)
            {
                var sb = new StringBuilder();
                foreach (var kv in Headers)
                {
                    if(string.IsNullOrWhiteSpace(kv.Key))
                        continue;

                    if(sb.Length != 0)
                        sb.Append("&");
                    sb.AppendFormat("{0}={1}", Uri.EscapeDataString(kv.Key), Uri.EscapeDataString(kv.Value));
                }

                map.Add(HeadersKey, sb.ToString());
            }

            return map;
        }

        public static JobOptions FromJobDataMap(JobDataMap map)
        {
            var jobOptions = new JobOptions
            {
                Id = map.GetString(IdKey),
                Cron = map.GetString(CronKey),
                Host = map.GetString(HostKey),
                Path = map.GetString(PathKey),
                Port = map.GetIntValue(PortKey)
            };

            if (map.TryGetValue(HeadersKey, out var headers))
            {
                var nvc = HttpUtility.ParseQueryString((string)headers);

                var dict = new Dictionary<string, string>();

                foreach (string key in nvc.AllKeys)
                {
                    if(!string.IsNullOrWhiteSpace(key))
                        dict.Add(key, nvc[key]);
                }

                jobOptions.Headers = dict;
            }

            return jobOptions;
        }
    }
}
