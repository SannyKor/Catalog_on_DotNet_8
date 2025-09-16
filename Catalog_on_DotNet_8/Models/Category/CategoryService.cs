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
        public List<Category> GetCategoriesTree()
        {
            var categories = _dbContext.Categories.ToList();
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

    }
}
