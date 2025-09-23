using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog_on_DotNet
{
    public class CategoryService
    {
        private readonly CatalogDbContext _dbContext;
        public CategoryService(CatalogDbContext dbContext) 
        { 
        _dbContext = dbContext;
        }
        
        public List<Category> GetAllCategories()
        {
            return _dbContext.Categories.ToList();
        }
        public Category? GetCategoryById(int id)
        {
            return _dbContext.Categories.FirstOrDefault(c => c.Id == id);
        }
        public bool AddCategory(Category category)
        { 
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
            return true;
        }
        public bool RemoveCategory(int id)
        {
            var category = _dbContext.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return false;
            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();
            return true;
        }
        public List<Unit> CategoryHasUnits(int categoryId)
        {
            return _dbContext.Categories
                .Where(c => c.Id == categoryId)
                .Include(c => c.Units)
                .SelectMany(c => c.Units)
                .ToList();
        }
        public List<Category> ShowSubCategories(int categoryId)
        {
            return _dbContext.Categories
                .Where(c => c.ParentId == categoryId)
                .ToList();
        }
        public List<Category> GetCategoriesTree(List<Category> categories)
        {            
            List<Category> rootCategories = new List<Category>();
            foreach (var category in categories)
            {
                if (category.ParentId == null)
                {
                    rootCategories.Add(category);
                }
                else 
                {
                    Category parentCategory = GetCategoryById(category.ParentId.Value);
                    parentCategory.SubCategories.Add(category);
                }
            }
            return rootCategories;
        }
        public void ShowCategoriesTree(List<Category> categories, string indent = "")
        {            
            foreach (var  category in categories)
            {                
                Console.WriteLine($"{indent} - {category.Name};");
                if (category.SubCategories.Any())
                {
                    ShowCategoriesTree(category.SubCategories, indent + "   ");// recurtion function to show subcategories
                }                
            }            
        }
        public List<Category> GetNoChildrenCategories(List<Category> categories)
        {            
            List<Category> hasNoChildrenCategories = new List<Category>();
            foreach (var category in categories)
            {
                if (!category.SubCategories.Any())
                {
                    hasNoChildrenCategories.Add(category);
                }
            }
            return hasNoChildrenCategories;
        }
        public void AddUnitToCategory(Unit unit)
        {
            List<Category> categories = GetAllCategories();
            List<Category> categoriesTree = GetCategoriesTree(categories);
                 

            while (true)
            {
                Console.WriteLine("Обрати головну категорію (n) \n створити нову категорію (y)");
                string? input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input) && input == "y" || categoriesTree.Count == 0)
                {
                    while (true)
                    {
                        Console.WriteLine("Введіть назву нової категорії:");
                        string? newCategoryName = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newCategoryName))
                        {
                            Category newCategory = new Category(newCategoryName);
                            AddCategory(newCategory);
                            Console.WriteLine($"Ви створили категорію: '{newCategoryName}'.");
                            categoriesTree.Add(newCategory);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Назва категорії не може бути порожньою.");
                        }
                    }
                }                
                else if (!string.IsNullOrEmpty(input) && input == "n")
                {
                    for (int i = 0; i < categoriesTree.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {categories[i].Name}");
                    }
                    Console.WriteLine("Оберіть категорію за номером:");
                    string? categoryNumberInput = Console.ReadLine();
                    if (int.TryParse(categoryNumberInput, out int number) && number > 0 && number >= categoriesTree.Count)
                    {
                        Console.WriteLine("Структура обраної категорії:");
                        Category selectedCategory = categoriesTree[number - 1];
                        Console.WriteLine($"{selectedCategory.Name}; \n");
                        //продовжити перевірку
                            while (true)
                            {
                                ShowCategoriesTree(selectedCategory.SubCategories, "  ");
                                Console.WriteLine("Вибрати категорію (введіть 'n') \nабо додати нову підкатегорію (введіть 'y'):");
                                string? subCategoryInput = Console.ReadLine();
                                if (!string.IsNullOrEmpty(subCategoryInput))
                                {
                                    if (subCategoryInput.ToLower() == "y")
                                    {
                                        Console.WriteLine("Введіть назву нової підкатегорії:");
                                        string? newSubCategoryName = Console.ReadLine();
                                        Console.WriteLine("Введіть назву категорії, до якої буде додана підкатегорія:");
                                        string? parentCategoryName = Console.ReadLine();
                                        if (!string.IsNullOrEmpty(newSubCategoryName) && !string.IsNullOrEmpty(parentCategoryName))
                                        {
                                            Category? parentCategory = categories.FirstOrDefault(c => c.Name == parentCategoryName);
                                            if (parentCategory != null)
                                            {
                                                Category newSubCategory = new Category(newSubCategoryName, parentCategory.Id);
                                                AddCategory(newSubCategory);
                                                parentCategory.SubCategories.Add(newSubCategory);
                                                Console.WriteLine($"Підкатегорія '{newSubCategoryName}' додана до категорії '{parentCategoryName}'.");
                                                
                                            }
                                            else
                                            {
                                                Console.WriteLine($"Категорія з назвою '{parentCategoryName}' не знайдена.");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Назва підкатегорії або батьківської категорії не може бути порожньою.");
                                        }
                                    }
                                    else if (subCategoryInput.ToLower() == "n")
                                    {
                                        Category selectedSubCategory = categories.First(c => c.Name == subCategoryInput);
                                        selectedSubCategory.Units.Add(unit);
                                        unit.Categories.Add(selectedSubCategory);
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Некоректне введення. Спробуйте ще раз.");
                                    }
                            }
                                else 
                                { Console.WriteLine("Некоректне введення. Спробуйте ще раз."); }
                            }
                        
                    }
                    else
                    { Console.WriteLine("Некоректне введення. Спробуйте ще раз."); }
                }
                else 
                { Console.WriteLine("Невірний вибір, спробуйте ще."); }
            }

        }
        public void AssignUnitToCategory(int unitId, List <Category> assignCategories)
        {
            var unit = _dbContext.Units.FirstOrDefault(u => u.Id == unitId);            
            if (unit != null && assignCategories != null)
            {
                foreach (var category in assignCategories)
                    if (!unit.Categories.Contains(category))
                    {
                        unit.Categories.Add(category);
                        category.Units.Add(unit);
                    }
                _dbContext.SaveChanges();
            }
        }

    }
}
