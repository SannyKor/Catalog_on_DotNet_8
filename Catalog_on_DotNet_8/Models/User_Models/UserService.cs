using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog_on_DotNet_8.Models.User_Models
{
    public class UserService
    {
        private string HashPassword(string password)
        {
            using (var sha256 = sha256.Create())
            {
                var hushedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hushedBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
