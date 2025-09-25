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
        User? currentUser = null;
        CategoryService categoryService;
        public ConsoleUI(Catalog catalog, UserService userService, CategoryService categoryService)
        {
            this.userService = userService;
            this.catalog = catalog;
            this.categoryService = categoryService;
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
            Unit addedUnit = catalog.AddUnit(name, description, prise, quantity, currentUser.Id);
            categoryService.AddUnitToCategory(addedUnit);
            while (true)
            {
                Console.WriteLine("Додати товар в іншу категорію? (y/n)");
                string? choise = Console.ReadLine();
                if (choise == "y")
                {
                    categoryService.AddUnitToCategory(addedUnit);
                }
                else if (choise == "n")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("невірний вибір, спробуйте ще раз");
                }
            }

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
                catalog.UpdateUnit(changedUnit, currentUser.Id);
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
            string? query = Console.ReadLine();
            int id = 0;
            if (!string.IsNullOrEmpty(query))
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
            Unit? unit = catalog.GetUnitById(id);
            if (unit == null)
            {
                Console.WriteLine("товар не знайдено\n");
            }
            else
            {
                UnitInfo(unit);
            }


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
        public void CategoriesMenu()
        {
            List<Category> categories = categoryService.GetAllCategories();
            while (true)
            {
                Console.WriteLine(
                    "Меню категорій: " +
                    "\n1. Показати всі категорії " +
                    "\n2. Додати категорію " +
                    "\n3. Видалити категорію " +
                    "\n4. Редагувати категорію " +
                    "\n5.  " +
                    "\n6.  " +
                    "\n7. Вийти з меню категорій" +
                    "\n\n Виберіть пункт меню за номером: ");
                string? input = Console.ReadLine();
                switch (input)
                {
                    case "1":                       
                        Console.WriteLine("Список усіх категорій:");
                        categoryService.ShowCategoriesTree(categoryService.GetCategoriesTree(categories));
                        break;

                    case "2":
                        Console.WriteLine("Додавання нової категорії:");
                        Console.WriteLine("Введіть назву нової категорії:");
                        string? newCategoryName = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newCategoryName))
                        {
                            Category newCategory = new Category(newCategoryName);
                            categoryService.AddCategory(newCategory);
                            Console.WriteLine($"Ви створили категорію: '{newCategoryName}'.");
                        }
                        else
                        {
                            Console.WriteLine("Назва категорії не може бути порожньою.");
                        }
                        break;

                    case "3":
                        Console.WriteLine("Видалення категорії:");
                        Console.WriteLine("Введіть назву категорії, яку бажаєте видалити:");
                        string? categoryToDelete = Console.ReadLine();
                        if (!string.IsNullOrEmpty(categoryToDelete))
                        {
                            var category = categories.FirstOrDefault(c => c.Name == categoryToDelete);
                            if (category != null)
                            {
                                if (!category.Units.Any() && !category.SubCategories.Any())
                                {
                                    categoryService.RemoveCategory(category.Id);
                                    Console.WriteLine($"Категорія '{categoryToDelete}' успішно видалена.");
                                }
                                else
                                {
                                    Console.WriteLine($"Категорія '{categoryToDelete}' не може бути видалена, оскільки вона містить підкатегорії або товари.");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Категорія з назвою '{categoryToDelete}' не знайдена.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Назва категорії не може бути порожньою.");
                        }
                        break;

                    case "4":
                        Console.WriteLine("Редагування категорії:");
                        Console.WriteLine("Введіть назву категорії, яку бажаєте редагувати:");
                        string? categoryToEdit = Console.ReadLine();
                        if (!string.IsNullOrEmpty(categoryToEdit))
                        {
                            var category = categories.FirstOrDefault(c => c.Name == categoryToEdit);
                            if (category != null)
                            {
                                Console.WriteLine("Введіть нову назву категорії:");
                                string? newName = Console.ReadLine();
                                if (string.IsNullOrEmpty(newName))
                                {
                                    category.Name = newName;
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Категорія з назвою '{categoryToEdit}' не знайдена.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Щось пішло не так введіть назву ще раз.");
                        }
                        break;

                    case "5":
                        break;

                    case "6":
                        break;

                    case "7":
                        Console.WriteLine("Вихід з меню категорій.");
                        return;
                }

            }
        }




        public void RunMainMenu()
        {            
            AuthService authService = new AuthService(userService);
            
            while (currentUser == null)
            {
                currentUser = authService.AuthRun();
                if (currentUser == null)
                {                    
                    return;
                }
            }
            Console.Clear();
            Console.WriteLine($"\nВітаємо, {currentUser.Name}!\n");

            List<MenuItem> menu = new List<MenuItem>
            {
                new MenuItem ("Додати новий товар", new List <UserRole>  {UserRole.Admin}, CatalogAction.AddUnit),
                new MenuItem("Видалити товар", new List <UserRole>  {UserRole.Admin}, CatalogAction.DeleteUnit),
                new MenuItem("Змінити інформацію про товар", new List <UserRole>  {UserRole.Admin, UserRole.Manager}, CatalogAction.ChangeUnitInfo),
                new MenuItem("Вивести інформацію про товар", new List <UserRole>  {UserRole.Admin, UserRole.Manager, UserRole.User}, CatalogAction.ShowUnit),
                new MenuItem("Показати весь каталог", new List <UserRole>  {UserRole.Admin, UserRole.Manager, UserRole.User}, CatalogAction.ShowAllUnits),
                new MenuItem("Показати рух кількості по товару", new List <UserRole>  {UserRole.Admin, UserRole.Manager}, CatalogAction.ShowQuantityHistory),
                new MenuItem("Знайти по назві або частині назви", new List <UserRole>  {UserRole.Admin, UserRole.Manager, UserRole.User}, CatalogAction.FindUnit),
                new MenuItem("Меню категорій", new List < UserRole > { UserRole.Admin}, CatalogAction.CategoriesMenu),
                new MenuItem("Вийти", new List < UserRole > { UserRole.Admin, UserRole.Manager, UserRole.User }, CatalogAction.Exit)                
            };

            
            
            while (true)
            {
                Console.WriteLine("Головне меню:");
                menu = menu.Where(m => m.AllowedRoles.Contains(currentUser.Role)).ToList();
                for (int i = 0; i < menu.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {menu[i].Title}");
                }
                Console.WriteLine("виберіть один із варіантів: ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= menu.Count)
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
                        case CatalogAction.CategoriesMenu:
                            CategoriesMenu();
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
