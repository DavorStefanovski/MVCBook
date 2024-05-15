using Microsoft.AspNetCore.Mvc.Rendering;
using MVCBookAesthetic.Models;
using System.Collections.Generic;

namespace MVCBookAesthetic.ViewModels
{
    public class BookGenreEditViewModel
    {
        public Book Book { get; set; }
        public IEnumerable<int>? SelectedGenres { get; set; }
        public IEnumerable<SelectListItem>? GenreList { get; set; }

    }
}
