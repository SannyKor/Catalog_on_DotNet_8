using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog_on_DotNet
{
    internal class AuthService
    {
        private readonly UserService _userService;
        public AuthService(UserService UserService)
        {
            _userService = UserService;
        }
        public bool IsAuthenticated(string username, string password)
        {
            // Simulate authentication logic
            return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
        }
        public void Logout()
        {
            // Simulate logout logic
            Console.WriteLine("User logged out successfully.");
        }
        
        public void Register(string username, string password)
        {
            // Simulate user registration logic
            Console.WriteLine($"User {username} registered successfully.");
        }
    }
}
