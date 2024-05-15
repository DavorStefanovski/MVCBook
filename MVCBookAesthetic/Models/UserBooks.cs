using System.ComponentModel.DataAnnotations;

namespace MVCBookAesthetic.Models
{
    public class UserBooks
    {
        public int Id { get; set; }

        [Display(Name = "Book")]
        public int? BookId { get; set; }
        public Book? Book { get; set; }

        [StringLength(450)]
        public string AppUser { get; set; }

    }
}
