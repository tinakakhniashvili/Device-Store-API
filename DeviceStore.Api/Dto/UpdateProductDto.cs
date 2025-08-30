﻿namespace DeviceStore.Dto
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = default!;
        public double Price { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
    }
}