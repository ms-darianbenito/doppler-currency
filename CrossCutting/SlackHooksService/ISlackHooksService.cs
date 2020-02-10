using System.Net.Http;
using System.Threading.Tasks;

namespace CrossCutting.SlackHooksService
{
    public interface ISlackHooksService
    {
        public Task<HttpResponseMessage> SendNotification(HttpClient httpClient, string message = null);
    }
}
