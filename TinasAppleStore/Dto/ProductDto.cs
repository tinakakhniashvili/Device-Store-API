using System.ComponentModel.DataAnnotations;

namespace TinasAppleStore.Dto
{
    public class ProductDto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The 'Name' field is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The 'Price' field is required.")]
        public double Price { get; set; }

        public string Description { get; set; }

    }
}
