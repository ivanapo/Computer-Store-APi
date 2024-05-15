using System.ComponentModel.DataAnnotations;

namespace ComputerStore.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
        public  decimal  Price { get; set; }
        public int Quantity { get; set; }
        public ICollection<Category> ProductCategories { get; set; }

    } 
}
