using ComputerStore.DTO;
using ComputerStore.Models;

namespace ComputerStore.Interfaces
{
    public interface ICategoryInterface
    {
        ICollection<CategoryDTO> GetCategories();
        Category GetCategory(int id);
        ICollection<Product> GetProductByCategory(int categoryId);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(int id);
        bool CategoryExists(int id);
        bool Save();
    }
}
