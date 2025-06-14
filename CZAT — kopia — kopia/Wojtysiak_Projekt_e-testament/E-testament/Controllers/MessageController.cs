using E_testament.Data;
using E_testament.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_testament.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MessageController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Błąd autoryzacji.");

            var messages = await _context.Messages
                .Where(m => m.SenderId == userId)
                .Select(m => new
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderName = m.Sender.Username,
                    SentAt = m.SentAt
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] MessageDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Błąd autoryzacji.");

            var sender = await _context.Users.FindAsync(userId);
            if (sender == null)
            {
                sender = new User { Id = userId, Username = dto.SenderName ?? "Anonymous" };
                _context.Users.Add(sender);
                await _context.SaveChangesAsync();
            }

            var message = new Message
            {
                Content = dto.Content,
                SenderId = userId,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(new { message.Id });
        }
    }

    public class MessageDto
    {
        public string Content { get; set; } = string.Empty;
        public string SenderName { get; set; } = "Anonymous"; // Dodane dla nazwy użytkownika
    }
}