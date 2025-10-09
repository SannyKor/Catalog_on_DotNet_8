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
            int quantity;
            while (true)
            {
                string? quantityInput = Console.ReadLine();
                
                if (!string.IsNullOrEmpty(quantityInput) && int.TryParse(quantityInput, out quantity))
                    break;
                else
                    Console.WriteLine("щось пішло не так, спробуйте ще");
            }           
            Console.WriteLine("введіть ціну: ");
            double prise;
            while (true)
            {
                string? priseInput = Console.ReadLine();

                if (!string.IsNullOrEmpty(priseInput) && double.TryParse(priseInput, out prise))                
                    break;                
                else
                    Console.WriteLine("щось пішло не так, спробуйте ще");                
            }
            Console.WriteLine("введіть опис: ");
            string? description = Console.ReadLine();
            Console.WriteLine($"name: {name} description: {description}");
            Unit addedUnit = catalog.AddUnit(name, description, prise, quantity, currentUser.Id);
            AddUnitToCategory(addedUnit.Id);
        }
        public int GetUnitId()
        {
            Console.WriteLine("введіть артикул: ");
            string? putId = Console.ReadLine();
            if (!string.IsNullOrEmpty(putId) && int.TryParse(putId, out int id))
            {
                Unit? unit = catalog.GetUnitById(id);
                if (unit != null)
                {
                    return unit.Id;
                }
                else
                {
                    Console.WriteLine("товар не знайдено\n");
                    return -1;
                }
            }
            else 
            {
            Console.WriteLine("Здається ви залишили поле пустим.");
                return -1;
            }
        }
        public void AddUnitToCategory(int unitId)
        {
            while (true)
            { 
            Console.WriteLine("Виберіть категорію зі списку до якої буде додано товар: ");
                categoryService.ShowCategoriesTree(categoryService.GetCategoriesTree(categoryService.GetAllCategories()));
                string? categoryName = Console.ReadLine();
                if (!string.IsNullOrEmpty(categoryName) && unitId > 0)
                {
                    if (categoryService.AssignUnitToCategory(unitId, categoryName))
                        Console.WriteLine($"Товар додано в категорію '{categoryName}'");
                    else
                        Console.WriteLine($"Категорія з назвою '{categoryName}' не знайдена. \n " +
                                          $"Переконайтесь в правильності вводу.");
                }
                else if (unitId < 0)
                    Console.WriteLine("Товару з таким артикулом не існує.");
                else                
                    Console.WriteLine("Назва категорії не може бути порожньою.");                
                
                Console.WriteLine("Додати товар в іншу категорію? (y/n)");
                string? choise = Console.ReadLine();

                if (choise == "y")
                    continue;
                else if (choise == "n")
                    break;
                else
                    Console.WriteLine("невірний вибір, спробуйте ще раз");
            }            
        }
        public void CategoriesInUnit(int unitId)
        {
            List<Category> categories = catalog.GetCategoriesInUnit(unitId);
            if (categories.Count > 0)
            {
                
                Console.WriteLine("Список категорій до яких належить товар: ");
                foreach (var category in categories)
                {
                    Console.WriteLine($"{categories.IndexOf(category) + 1}. {category.Name}");
                }
            }
            else
            {
                Console.WriteLine("Товар не належить жодній категорії або товару з таким артикулом не існує.");
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
        public void ShowUnitsInCategory()
        {
            Console.WriteLine("Введіть назву категорії: \n");
            string? categoryName = Console.ReadLine();
            
            if (!string.IsNullOrEmpty(categoryName))
            {
                var category = categoryService.GetCategoryByName(categoryName);
                if (category != null)
                {                   
                        List <Unit> unitsInCategory = categoryService.GetUnitsInCategoryIncludingSubCategories(category);  
                    foreach (var unit in unitsInCategory)
                    {
                        Console.WriteLine($"{unitsInCategory.IndexOf(unit) + 1}.\n");
                        UnitInfo(unit);
                    }
                }
                else
                {
                    Console.WriteLine($"Категорія з назвою '{categoryName}' не знайдена. \n " +
                                      $"Переконайтесь в правильності вводу.");
                    return;
                }
                
            }
            else
            {
                Console.WriteLine("Назва категорії не може бути порожньою.");
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
                        bool validInput = false;
                        do
                        {
                            Console.WriteLine("Створити головну категорію натисніть(n), додати підкатегорію натисніть (y)");
                            ConsoleKeyInfo key = Console.ReadKey(true);
                            Console.WriteLine();
                            char choise = char.ToLower(key.KeyChar);
                            if (choise == 'n')
                            {
                                while (true)
                                {
                                    Console.WriteLine("Введіть назву нової категорії:");
                                    string? newCategoryName = Console.ReadLine();
                                    if (!string.IsNullOrEmpty(newCategoryName))
                                    {
                                        Category newCategory = new Category(newCategoryName);
                                        categoryService.AddCategory(newCategory);
                                        categories = categoryService.GetAllCategories();
                                        Console.WriteLine($"Ви створили категорію: '{newCategoryName}'.");
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Назва категорії не може бути порожньою.");
                                    }                                    
                                }
                                validInput = true;
                            }
                            else if (choise == 'y')
                            {
                                while (true)
                                {
                                    Console.WriteLine("Введіть назву нової підкатегорії:");
                                    string? newSubCategoryName = Console.ReadLine();
                                    

                                    Console.WriteLine("Введіть назву категорії до якої буде додано підкатегорію:");
                                    string? parentCategoryName = Console.ReadLine();
                                   

                                    if (!string.IsNullOrEmpty(newSubCategoryName) && !string.IsNullOrEmpty(parentCategoryName))
                                    {
                                        bool exists = categories.Any(c => String.Equals(c.Name, newSubCategoryName, StringComparison.OrdinalIgnoreCase));
                                        Category? parentCategory = categories.FirstOrDefault(c => c.Name == parentCategoryName);
                                        if (!exists && parentCategory != null)
                                        {
                                            Category newSubCategory = new Category(newSubCategoryName, parentCategory.Id);
                                            categoryService.AddCategory(newSubCategory);
                                            categories = categoryService.GetAllCategories();
                                            Console.WriteLine($"Ви створили категорію: '{newSubCategoryName}'.");
                                            break;
                                        }
                                        else if (exists)
                                        {
                                            Console.WriteLine($"Категорія з назвою '{newSubCategoryName}' вже існує. Введіть іншу назву.");
                                        }
                                        else if (parentCategory == null)
                                        {
                                            Console.WriteLine($"Категорія з назвою '{parentCategoryName}' не знайдена. \n " +
                                                              $"Переконайтесь в правильності вводу.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Назва категорії та назва батьківської категорії не можуть бути порожніми.");
                                    }
                                }                                
                                validInput = true;
                            }
                            else
                            {
                                Console.WriteLine("невірний вибір, спробуйте ще раз");
                            }

                        } while (!validInput);
                        
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
                                if (!string.IsNullOrEmpty(newName))
                                {
                                    if (categoryService.ChangeCategory(category.Id, newName))
                                    {
                                        Console.WriteLine($"Категорія '{categoryToEdit}' успішно перейменована на '{newName}'.");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Категорія з назвою '{newName}' вже існує. Введіть іншу назву.");
                                    }
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
                new MenuItem("Додати товар до ктегорії", new List<UserRole> {UserRole.Admin}, CatalogAction.AddUnitToCategory),
                new MenuItem("Ктегорії до яких належить товар", new List<UserRole> {UserRole.Admin}, CatalogAction.CategoriesInUnit),
                new MenuItem("Видалити товар", new List <UserRole> {UserRole.Admin}, CatalogAction.DeleteUnit),
                new MenuItem("Змінити інформацію про товар", new List <UserRole>  {UserRole.Admin, UserRole.Manager}, CatalogAction.ChangeUnitInfo),
                new MenuItem("Вивести інформацію про товар", new List <UserRole>  {UserRole.Admin, UserRole.Manager, UserRole.User}, CatalogAction.ShowUnit),
                new MenuItem("Показати весь каталог", new List <UserRole>  {UserRole.Admin, UserRole.Manager, UserRole.User}, CatalogAction.ShowAllUnits),
                new MenuItem("Показати рух кількості по товару", new List <UserRole>  {UserRole.Admin, UserRole.Manager}, CatalogAction.ShowQuantityHistory),
                new MenuItem("Знайти по назві або частині назви", new List <UserRole>  {UserRole.Admin, UserRole.Manager, UserRole.User}, CatalogAction.FindUnit),
                new MenuItem("Меню категорій", new List < UserRole > { UserRole.Admin}, CatalogAction.CategoriesMenu),
                new MenuItem("Розгорнути каталог по категоріям", new List<UserRole> {UserRole.Admin, UserRole.Manager, UserRole.User}, CatalogAction.ShowAllCategories),
                new MenuItem("Всі товари в категорії", new List<UserRole>{UserRole.Admin, UserRole.Manager, UserRole.User}, CatalogAction.AllUnitsInCategory),
                new MenuItem("Вийти", new List <UserRole> { UserRole.Admin, UserRole.Manager, UserRole.User }, CatalogAction.Exit)
                
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
                        case CatalogAction.AddUnitToCategory:
                            AddUnitToCategory(GetUnitId());
                            break;
                        case CatalogAction.CategoriesInUnit:
                            CategoriesInUnit(GetUnitId());
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
                        case CatalogAction.ShowAllCategories:
                            categoryService.ShowCategoriesTree(categoryService.GetCategoriesTree(categoryService.GetAllCategories()));
                            break;
                        case CatalogAction.AllUnitsInCategory:
                            ShowUnitsInCategory();
                            break;
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
