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
        public Category? GetCategoryByName(string name)
        {
            return _dbContext.Categories.FirstOrDefault(c => c.Name == name);
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
        public List<Unit> ShowUnitsInCategory(string categoryName)
        {
            return _dbContext.Categories
                .Where(c => c.Name == categoryName)
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
               
        
        public bool AssignUnitToCategory(int unitId, string assignCategoryName)
        {
            var unit = _dbContext.Units.FirstOrDefault(u => u.Id == unitId);
            var assignCategory = _dbContext.Categories.FirstOrDefault(c => c.Name == assignCategoryName);
            if (unit != null && assignCategory != null)
            {             
                unit.Categories.Add(assignCategory);
                assignCategory.Units.Add(unit);                    
                _dbContext.SaveChanges();
                return true;
            }
            return false;
        }

    }
}
