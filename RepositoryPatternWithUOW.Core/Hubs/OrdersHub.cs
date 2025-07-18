﻿namespace Otlob.Core.Hubs
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
