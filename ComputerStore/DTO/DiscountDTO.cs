namespace ComputerStore.DTO
{
    public class DiscountDTO
    {
        public ICollection<string> Products { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
