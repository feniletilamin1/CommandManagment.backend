using CommandManagment.backend.Data;
using CommandManagment.backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommandManagment.backend.Helpers
{
    public class ContextHelper
    {
        readonly AppDbContext _context;

        public ContextHelper(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Team> GetTeamById(int id)
        {
            return await _context.Teams.FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
