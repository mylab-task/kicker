using MyLab.ApiClient;

namespace MyLab.Task.Scheduler
{
    [Api("process")]
    interface ITaskAppContract
    {
        [Post]
        System.Threading.Tasks.Task Start();
    }
}
