namespace MyLab.TaskKicker
{
    public interface ITaskKickerService
    {
        System.Threading.Tasks.Task<TaskKickResult> KickAsync(KickOptions kickOptions);
    }
}
