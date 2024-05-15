using Microsoft.AspNetCore.Mvc.Rendering;
using MVCBookAesthetic.Models;

namespace MVCBookAesthetic.ViewModels
{
    public class BookGenreCreateViewModel
    {
        public Book Book { get; set; }
        public IEnumerable<int>? SelectedGenres { get; set; }
        public IEnumerable<SelectListItem>? genresList {  get; set; }
    }
}
