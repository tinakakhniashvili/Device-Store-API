namespace DeviceStore.Dto
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}