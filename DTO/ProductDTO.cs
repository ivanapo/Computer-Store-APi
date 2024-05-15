namespace ComputerStore.DTO
{
    public class ProductDTO
    {
            public string Name { get; set; }
            public string Discription { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public ICollection<string> ProductCategories { get; set; }
        
    }
}
