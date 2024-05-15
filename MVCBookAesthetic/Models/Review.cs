using System.ComponentModel.DataAnnotations;

namespace MVCBookAesthetic.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Display(Name = "Book")]
        public int? BookId { get; set; }
        public Book? Book { get; set; }

        [StringLength(450)]
        public string AppUser { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }
        [Range(1,10)]
        public int? Rating { get; set; }
    }
}
