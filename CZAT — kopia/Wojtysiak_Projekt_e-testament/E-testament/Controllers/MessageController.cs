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
    public class MessageController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MessageController(AppDbContext context)
        {
            _context = context;
        }

        // POST: /api/message
        [HttpPost]
        [Authorize]
        public IActionResult SendMessage([FromBody] SendMessageDto dto)
        {
            var senderId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var message = new Message
            {
                SenderId = senderId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            return Ok(message);
        }

        // GET: /api/message
        [HttpGet]
        [Authorize]
        public IActionResult GetMessages()
        {
            var messages = _context.Messages
                .Include(m => m.Sender)
                .OrderBy(m => m.SentAt)
                .ToList();

            return Ok(messages);
        }

        public class SendMessageDto
        {
            public string Content { get; set; } = string.Empty;
        }
    }
}
