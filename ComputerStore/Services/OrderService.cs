using ComputerStore.Data;
using ComputerStore.DTO;
using ComputerStore.Interfaces;
using ComputerStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace ComputerStore.Services
{
    public class OrderService
    {
        private readonly IProductInterface _productInterface;
        private readonly DataContext _dbContext;

        public OrderService(IProductInterface productInterface, DataContext dataContext)
        {
            _productInterface = productInterface;
            _dbContext = dataContext;
        }

        public DiscountDTO CalculateDiscount(List<Order> products)
        {
            try
            {
                if (products.Count <= 0)
                {

                    throw new Exception("Enter Quantity bigger than 0");
                }

                if (products.Count == 1)
                {
                    var product = _dbContext.Products.FirstOrDefault(p => p.Name.Equals(products.First().Name));
                    if (products.First().Quantity > product.Quantity)
                    {
                        throw new Exception();
                    }

                    product.Quantity -= products.First().Quantity;
                    _dbContext.Update(product);
                    _dbContext.SaveChanges();

                    return new DiscountDTO
                    {
                        Products = new List<string> { product.Name },
                        TotalPrice = product.Price * products.First().Quantity
                    };
                }
                else
                {
                    var discountDTO = new DiscountDTO
                    {
                        Products = new List<string>(),
                        TotalPrice = 0
                    };

                    foreach (var product1 in products)
                    {
                        foreach (var product2 in products)
                        {
                            if (product1 != product2)
                            {
                                var p1 = _dbContext.Products
                                    .Include(p => p.ProductCategories)
                                    .Where(p => p.Name.Equals(product1.Name))
                                    .FirstOrDefault();

                                var p2 = _dbContext.Products
                                    .Include(p => p.ProductCategories)
                                    .Where(p => p.Name.Equals(product2.Name))
                                    .FirstOrDefault();

                                if (p1.Quantity < product1.Quantity)
                                {
                                    throw new Exception("We are out of stock.");
                                }

                                if (p1 == null || p2 == null)
                                {
                                    throw new Exception("There are no products with that name");
                                }

                                if (p1.ProductCategories.Any(p2.ProductCategories.Contains))
                                {
                                    p1.Price *= 0.95m;

                                }

                                p1.Quantity -= products.First().Quantity;
                                _dbContext.Update(p1);
                                _dbContext.SaveChanges();

                                discountDTO.Products.Add(p1.Name);
                                discountDTO.TotalPrice += p1.Price * products.First().Quantity;
                            }

                        }
                    }
                    return discountDTO;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while calculating discount.", ex);
            }
        }
    }
}