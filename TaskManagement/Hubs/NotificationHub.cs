using Microsoft.AspNetCore.SignalR;
using TaskManagement.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly TaskManagementContext dbContext;

        public NotificationHub(TaskManagementContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task SendNotificationToAll(string message)
        {
            await Clients.All.SendAsync("ReceivedNotification", message);
        }

        public async Task SendNotificationToClient(string message, string username)
        {
            var hubConnections = dbContext.HubConnections.Where(con => con.Username == username).ToList();
            foreach (var hubConnection in hubConnections)
            {
                if (hubConnection != null && Clients != null)
                {
                    await Clients.Client(hubConnection.ConnectionId).SendAsync("ReceivedPersonalNotification", message, username);
                }
            }
        }

        public async Task SendNotification(string userName, string message)
        {
            await Clients.User(userName).SendAsync("ReceiveNotification", message);
        }
        //public async Task SendNotificationToGroup(string message, string group)
        //{
        //    var hubConnections = dbContext.HubConnections.Join(dbContext.TblUser, c => c.Username, o => o.Username, (c, o) => new { c.Username, c.ConnectionId, o.Dept }).Where(o => o.Dept == group).ToList();
        //    foreach (var hubConnection in hubConnections)
        //    {
        //        string username = hubConnection.Username;
        //        await Clients.Client(hubConnection.ConnectionId).SendAsync("ReceivedPersonalNotification", message, username);
        //        //Call Send Email function here
        //    }
        //}

        public override Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            Clients.Caller.SendAsync("OnConnected", connectionId);
            return base.OnConnectedAsync();
        }

        public async Task SaveUserConnection(string username)
        {
            var connectionId = Context.ConnectionId;
            var hub = dbContext.HubConnections.Where(x => x.Username == username).FirstOrDefault();
            if (hub != null)
            {
                hub.ConnectionId = connectionId;
                dbContext.HubConnections.Update(hub);
            }
            else
            {
                HubConnection hubConnection = new HubConnection
                {
                    ConnectionId = connectionId,
                    Username = username
                };
                dbContext.HubConnections.Add(hubConnection);
            }
            await dbContext.SaveChangesAsync();
        }

        //public override Task OnDisconnectedAsync(Exception? exception)
        //{
        //    var hubConnection = dbContext.HubConnections.FirstOrDefault(con => con.ConnectionId == Context.ConnectionId);
        //    if(hubConnection != null)
        //    {
        //        dbContext.HubConnections.Remove(hubConnection);
        //        dbContext.SaveChangesAsync();
        //    }

        //    return base.OnDisconnectedAsync(exception);
        //}
    }
}
