using System.Threading.Tasks;

namespace CrossCutting.SlackHooksService
{
    public interface ISlackHooksService
    {
        public Task SendNotification(string message = null);
    }
}
