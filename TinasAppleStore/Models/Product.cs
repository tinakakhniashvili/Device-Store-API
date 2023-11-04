namespace TinasAppleStore.Models
{
    public class Product
    {
        public int productId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
