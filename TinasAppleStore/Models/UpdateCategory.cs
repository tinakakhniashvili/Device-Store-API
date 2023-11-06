namespace TinasAppleStore.Models
{
    public class UpdateCategory
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
