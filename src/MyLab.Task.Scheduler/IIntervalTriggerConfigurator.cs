using Quartz;

namespace MyLab.Task.Scheduler
{
    interface IIntervalTriggerConfigurator
    {
        ITriggerConfigurator Configure(ITriggerConfigurator triggerConfigurator, JobOptions jobOptions);
    }

    class IntervalTriggerConfigurator : IIntervalTriggerConfigurator
    {
        public ITriggerConfigurator Configure(ITriggerConfigurator triggerConfigurator, JobOptions jobOptions)
        {
            return triggerConfigurator.WithCronSchedule(jobOptions.Cron);
        }
    }
}
