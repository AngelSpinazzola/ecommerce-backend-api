﻿namespace EcommerceAPI.DTOs.Products
{
    public class ProductDto
    {
        public int Id { get; set; }                  
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }              
        public string Model { get; set; }             
        public string MainImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
    }
}
