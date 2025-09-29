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
                    FillSubCategories(category, categories);
                    rootCategories.Add(category);
                }                
            }
            return rootCategories;
        }
        private void FillSubCategories(Category parentCategory, List<Category> categories)
        {
            var subCategories = categories.Where(c => c.ParentId == parentCategory.Id).ToList();
            foreach (var category in subCategories)
            {
                if(!parentCategory.SubCategories.Contains(category))
                {
                    parentCategory.SubCategories.Add(category);
                }                
                FillSubCategories(category, categories); // recurtion function to fill subcategories
            }
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
        public List<Category> GetRootCategories()
        {
            List<Category> categories = GetAllCategories();
            List<Category> rootCategories = new List<Category>();
            foreach (var category in categories)
            {
                if (category.ParentId == null)
                {
                    rootCategories.Add(category);
                }
            }
            return rootCategories;
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
            List<Category> rootCategories = new List<Category>();
            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].ParentId == null)
                {
                    rootCategories.Add(categories[i]);

                }
            }
            //List<Category> categoriesTree = GetCategoriesTree(categories);


            while (true)
            {
                Console.WriteLine("Обрати головну категорію (n) \n створити нову категорію (y)");
                string? input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input) && input == "y")
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
                    
                    for(int i = 0; i < rootCategories.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {rootCategories[i].Name}");
                    }
                    Console.WriteLine("Оберіть категорію за номером:");
                    string? categoryNumberInput = Console.ReadLine();
                    if (int.TryParse(categoryNumberInput, out int number) && number > 0 && number >= rootCategories.Count)
                    {
                        Console.WriteLine("Структура обраної категорії:");                        
                        Category selectedCategory = rootCategories[number - 1];
                        Console.WriteLine($"{selectedCategory.Name};");
                        ShowCategoriesTree(selectedCategory.SubCategories, "  ");
                        
                        while (true)
                            {                                
                                Console.WriteLine("Вибрати категорію (введіть 'n') \nабо додати нову підкатегорію (введіть 'y'):");
                                string? InPutChoiсe = Console.ReadLine();
                                if (!string.IsNullOrEmpty(InPutChoiсe))
                                {
                                    if (InPutChoiсe.ToLower() == "y")
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
                                            break;
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
                                    else if (InPutChoiсe.ToLower() == "n")
                                    {
                                        //Category selectedSubCategory = categories.First(c => c.Name == subCategoryInput);
                                        //selectedSubCategory.Units.Add(unit);
                                        //unit.Categories.Add(selectedSubCategory);
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
