using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Otlob.Core.Hubs
{
    public class OrdersHub : Hub
    {        
        public async Task JoinGroup(string restaurantId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, restaurantId);
        }

        public async Task LeaveGroup(string restaurantId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, restaurantId);
        }
    }
}
