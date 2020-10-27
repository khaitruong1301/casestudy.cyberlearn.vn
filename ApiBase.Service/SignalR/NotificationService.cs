using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ApiBase.Service.SignalR
{
    public interface INotificationService
    {
        Task ConfirmAccount(string id, string fullname);
    }

    public class NotificationService : INotificationService
    {
        private IHubContext<AppHub> _hubContext;

        public NotificationService(IHubContext<AppHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task ConfirmAccount(string id, string fullname)
        {
            await _hubContext.Clients.All.SendAsync("ConfirmAccount", id, fullname);
        }
    }
}