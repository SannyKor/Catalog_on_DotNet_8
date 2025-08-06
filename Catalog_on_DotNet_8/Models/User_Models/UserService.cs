using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Catalog_on_DotNet;




namespace Catalog_on_DotNet
{
    public class UserService
    {
        private readonly CatalogDbContext _dbContext;
        public UserService(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        private string GenerateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var saltBytes = new byte[16];
                rng.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }
        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var hushedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return BitConverter.ToString(hushedBytes).Replace("-", "").ToLowerInvariant();
            }
        }
        public bool AddUser(string name, string email, string password)
        {

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return false; // Invalid input
            }
            if (_dbContext.Users.Any(u => u.Email == email))
            {
                return false; // User with this email already exists
            }
            if (password.Length < 6)
            {
                return false; // Password too short
            }

            var salt = GenerateSalt();
            var passwordHash = HashPassword(password, salt);
            email = email.ToLower();
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                PasswordHash = passwordHash,
                Salt = salt,
                Role = UserRole.User,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return true;
        }

    }
}
