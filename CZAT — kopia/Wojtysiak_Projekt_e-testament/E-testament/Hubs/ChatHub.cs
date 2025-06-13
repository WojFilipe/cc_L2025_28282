using E_testament.Data;
using E_testament.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace E_testament
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string userName, string message)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(message))
                return;

            // 🔍 Szukamy lub tworzymy użytkownika
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == userName);
            if (user == null)
            {
                user = new User { Name = userName };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // 💾 Zapisujemy wiadomość do bazy
            var msg = new Message
            {
                Content = message,
                SentAt = DateTime.UtcNow,
                SenderId = user.Id
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            // 📡 Wysyłamy do wszystkich
            await Clients.All.SendAsync("ReceiveMessage", userName, message);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}
