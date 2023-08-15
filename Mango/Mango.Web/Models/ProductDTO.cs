using Mango.Web.Utility;
using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1, 1000)]
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }
        [Range(1, 100)]
        public int Count { get; set; } = 1;
        [AllowedExtension(new string[] { ".jpg", ".png" })]
        [AllowedFileSize(1)]
        public IFormFile Image { get; set; }
    }
}
