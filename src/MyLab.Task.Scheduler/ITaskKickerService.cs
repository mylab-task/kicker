namespace MyLab.Task.Scheduler
{
    public interface ITaskKickerService
    {
        System.Threading.Tasks.Task KickAsync(KickOptions kickOptions);
    }
}
