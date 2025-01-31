using System.Security.Cryptography;
using System.Text;
using e_learning_app.Data;
using e_learning_app.Models;
using Microsoft.EntityFrameworkCore;

namespace e_learning_app.Services;

public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateUser(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                return null;
            }
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserById(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task CreateUser(User user)
        {
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                throw new ArgumentException("Hasło nie może być puste.");
            }

            user.Id = Guid.NewGuid();
            user.PasswordHash = HashPassword(user.PasswordHash);
            user.Role = user.Role ?? "Student";

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateUser(Guid id, User user)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return;

            existingUser.Username = user.Username;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;
            
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                existingUser.PasswordHash = HashPassword(user.PasswordHash);
            }

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hash = HashPassword(password);
            return hash == storedHash;
        }
    }