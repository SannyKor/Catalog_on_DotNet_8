using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Catalog_on_DotNet
{
    public class AuthService
    {
        private UserService _userService;
        public AuthService(UserService userService)
        {
            _userService = userService;
        }
        
        public static string ReadPassword()
        {
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(intercept: true);
                if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);
            return password.ToString();
        }
        private User? HandleLogin()
        {
            Console.Clear();
            Console.WriteLine("введіть email: ");
            string? email = Console.ReadLine();
            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("email не може бути порожнім, спробуйте ще раз");
                return null;
            }
            Console.WriteLine("введіть пароль: ");

            string? password = ReadPassword();
            bool userVerification = _userService.LoginUser(email, password);
            if (userVerification)
            {
               // Console.WriteLine("\nВітаємо, ви успішно увійшли в акаунт!");
                return  _userService.GetUserByEmail(email);
            }
            else
            {
                Console.WriteLine("невірний email або пароль, спробуйте ще раз");
                return null;
            }
        }
        private User? HandleRegistration()
        {
            Console.Clear();
            Console.WriteLine("введіть email для реєстрації: ");
            string? email = Console.ReadLine();
            Console.WriteLine("Додайте ім'я користувача: ");
            string? name = Console.ReadLine();
            if (string.IsNullOrEmpty(email)||string.IsNullOrEmpty(name))
            {
                Console.WriteLine("поля ім'я та email не можуть бути порожніми, спробуйте ще раз");
                return null;
            }
            else if (_userService.GetUserByEmail(email) != null)
            {
                Console.WriteLine("користувач з таким email вже існує, спробуйте ще раз");
                return null;
            }
            else
            {
                Console.WriteLine("введіть пароль: \t");
                string? password = ReadPassword();
                Console.WriteLine("повторіть пароль: \t");
                string? passwordRepeat = ReadPassword();
                if (password.Length < 6)
                {
                    Console.WriteLine("пароль має бути не менше 6 символів, спробуйте ще раз");
                    return null;
                }
                else if (password != passwordRepeat)
                {
                    Console.WriteLine("паролі не співпадають, спробуйте ще раз");
                    return null;
                }
                else
                {
                    bool userAdded = _userService.AddUser(name, email, password);
                    if (userAdded)
                    {
                        Console.WriteLine("користувача успішно зареєстровано");
                        return _userService.GetUserByEmail(email);
                    }
                    else
                    {
                        Console.WriteLine("щось пішло не так, спробуйте ще раз");
                        return null;
                    }
                }
            }
        }
        public User? AuthRun()
        {
            User? user = null;
            Console.WriteLine("вітаємо в каталозі товарів!");
            while (true)
            {
                Console.WriteLine(
                    "для входу в акаунт натисніть '1', " +
                    "\nдля реєстрації користувача натисніть '2', " +
                    "\nдля виходу натисніть 3: ");
                string? authChoice = Console.ReadLine();
                if (authChoice == "1")
                {
                    user = HandleLogin();
                    if (user != null)
                    {
                        //Console.WriteLine($"\nВітаємо, {user.Name}! Ви успішно увійшли в акаунт.");
                        return user; // Повертаємо користувача після успішного входу
                    }
                    else
                    {
                        Console.WriteLine("вхід не вдалося здійснити, спробуйте ще раз");
                        continue;
                    }
                }
                else if (authChoice == "2")
                {
                    user = HandleRegistration();
                    if (user != null)
                    {
                        Console.WriteLine($"\nВітаємо, {user.Name}! Ви успішно зареєстровані.");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("реєстрація не вдалася, спробуйте ще раз");
                        continue;
                    }

                }
                else if (authChoice == "3")
                {
                    Console.WriteLine("до побачення!");
                    return null;
                }
                else
                {
                    Console.WriteLine("невірний вибір, спробуйте ще раз");
                    continue;
                }
            }
        }
    }
}
