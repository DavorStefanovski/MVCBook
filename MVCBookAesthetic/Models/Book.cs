using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace MVCBookAesthetic.Models
{
    public class Book
    {
        public int Id { get; set; } 

        [StringLength(100, MinimumLength=3)]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Year published")]
        public int? YearPublished { get; set; }

        [Display(Name = "Number of pages")]
        public int? NumPages { get; set; }

        public string? Description { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string? Publisher { get; set; }

        [NotMapped]
        [Display(Name = "Front Page")]
        public IFormFile FrontPageFile { get; set; }
        public string? FrontPage { get; set; }

        [NotMapped]
        [Display(Name = "PDF File")]
        public IFormFile PdfFile { get; set; }
        public string? DownloadUrl { get; set; }


        [Display(Name = "Author")]
        public int? AuthorId { get; set; }
        public Author? Author { get; set; }

        public ICollection<BookGenre>? Genres { get; set; }
        public ICollection<Review>? Reviews{ get; set; }
        public ICollection<UserBooks>? Users { get; set; }
    }
}
