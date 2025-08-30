namespace DeviceStore.Dto
{
    public class ProductQuery
    {
        private const int MaxPageSize = 100;

        public int Page { get; set; } = 1;
        private int _pageSize = 20;
        public int PageSize { get => _pageSize; set => _pageSize = value > MaxPageSize ? MaxPageSize : value; }

        public string? Search { get; set; }
        public string? Sort { get; set; } = "name";

        // NEW:
        public int? CategoryId { get; set; }
        public string? Category { get; set; } // by name (case-insensitive)
    }
}