using MVCBookAesthetic.Models;
namespace MVCBookAesthetic.ViewModels
{
    public class BookReview
    {
        public string Title { get; set; }
        public int Id { get; set; } // This is the book ID
        public Review Review { get; set; } = new Review();
    }

}
