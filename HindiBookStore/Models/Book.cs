using System.ComponentModel.DataAnnotations;

namespace HindiBookStore.Models
{
    public class Book
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Author { get; set; } = string.Empty;
        
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;
        
        public string? CoverImagePath { get; set; }
        
        public string? PdfFilePath { get; set; }
        
        [StringLength(100)]
        public string Genre { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
