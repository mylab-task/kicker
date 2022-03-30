namespace MyLab.Task.Scheduler
{
    public interface ITaskKickerService
    {
        System.Threading.Tasks.Task<TaskKickResult> KickAsync(KickOptions kickOptions);
    }
}
