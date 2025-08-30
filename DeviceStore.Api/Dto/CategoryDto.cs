namespace DeviceStore.Dto
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public int ProductCount { get; set; }
    }
}