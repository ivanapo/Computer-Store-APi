using ComputerStore.DTO;
using ComputerStore.Models;

namespace ComputerStore.Interfaces
{
    public interface IProductInterface
    {
        ICollection<Product> GetProducts();
        Product GetProduct(int id);
        Product GetProductByName(string name);
        bool CreateProduct(ProductDTO product);
        bool ProductExists(int id);
        bool DeleteProduct(int id);
        bool UpdateProduct(Product product);
        bool Save();
    }
}
