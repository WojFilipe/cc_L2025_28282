using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace E_testament.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($" Nowe połączenie: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($" Rozłączono: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string username, string message)
        {
            try
            {
                Console.WriteLine($" {username}: {message}");
                await Clients.All.SendAsync("ReceiveMessage", username, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Błąd w SendMessage: {ex.Message}");
                throw;
            }
        }
    }
}