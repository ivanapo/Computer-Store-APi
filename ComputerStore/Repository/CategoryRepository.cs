using ComputerStore.Data;
using ComputerStore.DTO;
using ComputerStore.Interfaces;
using ComputerStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ComputerStore.Repository
{
    public class CategoryRepository : ICategoryInterface
    {
        private DataContext _context;
        public CategoryRepository(DataContext context)
        {
            _context = context;
        }
        public ICollection<CategoryDTO> GetCategories()
        {
            var categories = _context.Categories.ToList();
            var categoryDTOs = new List<CategoryDTO>();

            foreach (var category in categories)
            {
                var categoryDTO = new CategoryDTO
                {
                    Name = category.Name,
                    Description = category.Description,
                };

                categoryDTOs.Add(categoryDTO);
            }

            return categoryDTOs;
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.Where(e => e.CategoryId == id).FirstOrDefault();
        }

        public ICollection<Product> GetProductByCategory(int categoryId)
        {
            return _context.Products
                   .Include(p => p.ProductCategories)
                   .Where(p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId))
                   .ToList();
        }
        public bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.CategoryId == id);
        }

        public bool CreateCategory(Category category)
        {
            _context.Add(category);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
        public bool UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            return Save(); 
        }
        public bool DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(p => p.CategoryId == id);
            if (category == null)
            {
                return false;
            }

            _context.Categories.Remove(category);
            return Save();
        }
    }
}
