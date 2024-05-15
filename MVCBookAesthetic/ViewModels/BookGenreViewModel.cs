using Microsoft.AspNetCore.Mvc.Rendering;
using MVCBookAesthetic.Models;
using System.Collections.Generic;

namespace MVCBookAesthetic.ViewModels
{
    public class BookGenreViewModel
    {
        public IList<Book> Books { get; set; }
        public SelectList Genres { get; set; }
        public string searchGenre { get; set; }
        public string searchTitle { get; set; }
        public string searchAuthor {  get; set; }
    }
}
