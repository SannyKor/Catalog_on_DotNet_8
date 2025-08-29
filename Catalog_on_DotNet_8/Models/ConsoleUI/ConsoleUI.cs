using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog_on_DotNet;

namespace Catalog_on_DotNet
{
    public class ConsoleUI
    {
        private Catalog catalog;
        UserService userService;

        public ConsoleUI(Catalog catalog, UserService userService)
        {
            this.userService = userService;
            this.catalog = catalog;
        }


        public void CreateNewUnit()
        {
            Console.WriteLine("введіть ім'я: ");
            string? name = Console.ReadLine();
            Console.WriteLine("введіть кількість: ");
            string? quantityInput = Console.ReadLine();
            int quantity;
            while(true)
            { 
                if (!string.IsNullOrEmpty(quantityInput) && int.TryParse(quantityInput, out quantity))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("щось пішло не так, спробуйте ще");
                }
            }           
            Console.WriteLine("введіть ціну: ");
            double prise = double.Parse(Console.ReadLine());
            Console.WriteLine("введіть опис: ");
            string? description = Console.ReadLine();
            Console.WriteLine($"name: {name} description: {description}");
            catalog.AddUnit(name, description, prise, quantity);
        }

        public void RemoveUnit()
        {
            Console.WriteLine("введіть артикул товару для видалення: ");
            int id = int.Parse(Console.ReadLine());

            if (catalog.RemoveUnit(id))
            {
                Console.WriteLine("Товар видалено\n");
            }
            else
            {
                Console.WriteLine("Товар не знайдено\n");
            }
        }

        public void ChangeUnitInfo()
        {
            int id;

            Console.WriteLine("введіть артикул: ");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("невіний формат, спробуйте ще раз");
            }

            Unit? unit = catalog.GetUnitById(id);

            if (unit == null)
            {
                Console.WriteLine("товар не знайдено\n");
                return;
            }
            else
            {
                Unit changedUnit = new Unit(id)
                {
                    Name = unit.Name,
                    Description = unit.Description,
                    Price = unit.Price,
                    Quantity = unit.Quantity,
                    AddedDate = unit.AddedDate,
                    QuantityHistory = unit.QuantityHistory
                };
                Console.WriteLine("введіть нове ім'я або enter щоб продовжити: ");
                string? name = Console.ReadLine();
                if (!string.IsNullOrEmpty(name))
                {

                    if (catalog.Units.Any(u => u.Name == name))
                    {
                        Console.WriteLine("товар з таким ім'ям вже існує. введіть інше ім'я або enter щоб продовжити\n");
                        return;
                    }
                    else
                    {
                        changedUnit.Name = name;
                    }
                }

                Console.WriteLine("введіть кількість: ");

                if (int.TryParse(Console.ReadLine(), out int parsedQuantity))
                {
                    changedUnit.Quantity = parsedQuantity;
                }

                Console.WriteLine("змініть ціну або натисніть enter щоб продовжити: ");
                if (double.TryParse(Console.ReadLine(), out double parsedPrice))
                {
                    changedUnit.Price = parsedPrice;
                }

                Console.WriteLine("введіть новий опис або enter щоб продовжити без змін: ");
                string? description = Console.ReadLine();
                if (!string.IsNullOrEmpty(description))
                {
                    changedUnit.Description = description;
                }
                catalog.UpdateUnit(changedUnit);
            }

        }
        public void UnitInfo(Unit unit)
        {
            Console.WriteLine($"артикул: \t{unit.Id}");
            Console.WriteLine($"назва:\t\t{unit.Name}");
            Console.WriteLine($"кількість: \t{unit.Quantity}");
            Console.WriteLine($"опис: \t\t{unit.Description}");
            Console.WriteLine($"ціна: \t\t{unit.Price}\n");
        }
        public void ShowUnitInfo()
        {
            Console.WriteLine("введіть артикул: ");
            int id = int.Parse(Console.ReadLine());
            Unit? unit = catalog.GetUnitById(id);
            if (unit == null)
            {
                Console.WriteLine("товар не знайдено\n");
            }

            UnitInfo(unit);


        }
        public void ShowAllUnitsInfo(IEnumerable<Unit> Units)
        {
            if (catalog.Units.Count == 0)
            {
                Console.WriteLine("в каталозі ще немає доданих товарів. натисніть 'enter' для продовження\n");
            }
            else
            {
                Console.WriteLine("ваш каталог: ");
                foreach (Unit unit in Units)
                {
                    UnitInfo(unit);
                }
            }
        }
        public void ShowUnitQuantityHistory()
        {
            Console.WriteLine("введіть артикул: ");
            string? query = Console.ReadLine();
            int id = -1;
            if (query != null)
            {
                if (!int.TryParse(query, out id))
                { 
                    Console.WriteLine("Невірний формат, спробуйте ще раз");
                    return;
                }
            }
            else
            {
                Console.WriteLine("здається ви нічого не ввели");
                return;
            }
                //int id = int.Parse(Console.ReadLine());
                List<Unit.SaveQuantityChange> quantityHistory = new List<Unit.SaveQuantityChange>();
            quantityHistory = catalog.GetUnitQuantityHistory(id);
            Console.WriteLine("\nартикул:\tкількість:\tчас:");

            foreach (var quantity in quantityHistory)
            {
                Console.WriteLine($"{quantity.UnitId}\t\t{quantity.NewUnitQuantity}\t\t{quantity.DateOfChange}");
            }
            Console.WriteLine("\n");
        }
        public void FindUnitByName()
        {
            Console.WriteLine("введіть запит:");
            string? query = Console.ReadLine();
            List<Unit> found = catalog.FindUnit(query);

            if (found.Count > 0)
            {
                foreach (var unit in found)
                {
                    UnitInfo(unit);
                }
            }
            else
            {
                Console.WriteLine("товар за запитом відсутній");
            }
        }
        

        
        
        public void RunMainMenu()
        {            
            AuthService authService = new AuthService(userService);
            User? currentUser = null;
            while (currentUser == null)
            {
                currentUser = authService.AuthRun();
            }
            Console.WriteLine($"Вітаємо, {currentUser.Name}!\n");

            List<MenuItem> menu = new List<MenuItem>
            {
                new MenuItem ("Додати новий товар", new List <UserRole>  {UserRole.Admin}, CatalogAction.AddUnit),
                new MenuItem("Видалити товар", new List <UserRole>  {UserRole.Admin}, CatalogAction.DeleteUnit),
                new MenuItem("Змінити інформацію про товар", new List <UserRole>  {UserRole.Admin, UserRole.Manager}, CatalogAction.ChangeUnitInfo),
                new MenuItem("Вивести інформацію про товар", new List <UserRole>  {UserRole.Admin, UserRole.Manager, UserRole.User}, CatalogAction.ShowUnit),
                new MenuItem("Показати весь каталог", new List <UserRole>  {UserRole.Admin, UserRole.Manager, UserRole.User}, CatalogAction.ShowAllUnits),
                new MenuItem("Показати рух кількості по товару", new List <UserRole>  {UserRole.Admin, UserRole.Manager}, CatalogAction.ShowQuantityHistory),
                new MenuItem("Знайти по назві або частині назви", new List <UserRole>  {UserRole.Admin, UserRole.Manager, UserRole.User}, CatalogAction.FindUnit),
                new MenuItem("Вийти", new List < UserRole > { UserRole.Admin, UserRole.Manager, UserRole.User }, CatalogAction.Exit)
            };

            Console.WriteLine("Головне меню:");
            menu = menu.Where(m => m.AllowedRoles.Contains(currentUser.Role)).ToList();
            for (int i = 0; i < menu.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {menu[i].Title}");
            }
            
            while (true)
            {
                Console.WriteLine("виберіть один із варіантів: ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice < menu.Count)
                {
                    var selectedAction = menu[choice - 1];
                    switch (selectedAction.Action)
                    {
                        case CatalogAction.AddUnit:
                            CreateNewUnit();
                            break;
                        case CatalogAction.DeleteUnit:
                            RemoveUnit();
                            break;
                        case CatalogAction.ChangeUnitInfo:
                            ChangeUnitInfo();
                            break;
                        case CatalogAction.ShowUnit:
                            ShowUnitInfo();
                            break;
                        case CatalogAction.ShowAllUnits:
                            ShowAllUnitsInfo(catalog.Units);
                            break;
                        case CatalogAction.ShowQuantityHistory:
                            ShowUnitQuantityHistory();
                            break;
                        case CatalogAction.FindUnit:
                            FindUnitByName();
                            break;
                        case CatalogAction.Exit:
                            return;
                        default:
                            Console.WriteLine("невірний вибір. спробуйте ще раз\n");
                            break;
                    }

                }
            }



            /*while (true)
            {
                Console.WriteLine("виберіть один із варіантів: " +
                    "\n1. додати новий товар; " +
                    "\n2. видалити товар; " +
                    "\n3. змінити інформацію про товар; " +
                    "\n4. вивести інформацію про товар;" +
                    "\n5. показати весь каталог;" +
                    "\n6. показати рух кількості по товару;" +
                    "\n7. знайти по назві або частині назви;" +
                    "\n8. вийти;\n");

                string? choise = Console.ReadLine();
                switch (choise)
                {
                    case "1":
                        CreateNewUnit();
                        break;
                    case "2":
                        RemoveUnit();
                        break;
                    case "3":
                        ChangeUnitInfo();
                        break;
                    case "4":
                        ShowUnitInfo();
                        break;
                    case "5":
                        ShowAllUnitsInfo(catalog.Units);
                        break;
                    case "6":
                        ShowUnitQuantityHistory();
                        break;
                    case "7":
                        FindUnitByName();
                        break;
                    case "8":

                        return;
                    default:
                        Console.WriteLine("невірний вибір. спробуйте ще раз\n");
                        break;
                }
            }*/
        }

    }
}
