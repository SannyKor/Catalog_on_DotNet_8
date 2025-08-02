using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Catalog_on_DotNet;




namespace Catalog_on_DotNet_8
{
    public class UserService
    {
        private readonly CatalogDbContext _dbContext;
        public UserService(CatalogDbContext dbContext)
        {
            var _dbContext = dbContext;
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
    }
}
