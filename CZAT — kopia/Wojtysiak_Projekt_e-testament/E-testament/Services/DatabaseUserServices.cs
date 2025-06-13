using E_testament.Data;
using E_testament.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace E_testament.Services
{
    public class DatabaseUserService
    {
        private readonly AppDbContext _context;

        public DatabaseUserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsValidUserAsync(string username, string password)
        {
            return await _context.Users.AnyAsync(u => u.Username == username && u.Password == password);
        }
    }
}
