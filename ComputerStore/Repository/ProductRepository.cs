using AutoMapper;
using ComputerStore.Data;
using ComputerStore.DTO;
using ComputerStore.Interfaces;
using ComputerStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ComputerStore.Repository
{
    public class ProductRepository : IProductInterface
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public ProductRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public bool CreateProduct(ProductDTO product)
        {
            var categories = _context
                .Categories
                .Where(c => product.ProductCategories.Contains(c.Name))
                .ToList();

            var newProduct = new Product
            {
                Name = product.Name,
                Discription = product.Discription,
                Price = product.Price,
                Quantity = product.Quantity,
                ProductCategories = categories
            };

            _context.Products.Add(newProduct);
            return Save();
        }


        public Product GetProduct(int id)
        {
            return _context.Products.Where(p => p.ProductId == id)
                .Include(p => p.ProductCategories)
                .FirstOrDefault();
        }

        public Product GetProductByName(string name)
        {
            return _context.Products.Where(p => p.Name == name).FirstOrDefault();
        }

        public ICollection<Product> GetProducts()
        {
            return _context.Products.OrderBy(p => p.ProductId).ToList();
        }

        public bool ProductExists(int id)
        {
            return _context.Products.Any(p => p.ProductId == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool DeleteProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);
            return Save();
        }

        public bool UpdateProduct(Product updatedProduct)
        {
            var productToUpdate = _context.Products.FirstOrDefault(p => p.ProductId == updatedProduct.ProductId);

            if (productToUpdate == null)
            {
                return false; 
            }

            productToUpdate.Name = updatedProduct.Name;
            productToUpdate.Discription = updatedProduct.Discription;
            productToUpdate.Price = updatedProduct.Price;
            productToUpdate.Quantity = updatedProduct.Quantity;

            return Save();
        }
    }
}
