namespace ProductService.Core.DTO
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool InStock { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime ImageCreatedDate { get; set; }
        public DateTime ImageModifiedDate { get; set; }
        public string? Title { get; set; }
        public string? PhysicalPath { get; set; }
        public int? CategoryId { get; set; }
    }
}
