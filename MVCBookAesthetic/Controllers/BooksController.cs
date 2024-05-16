using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MVCBookAesthetic.Data;
using MVCBookAesthetic.Models;
using MVCBookAesthetic.ViewModels;

namespace MVCBookAesthetic.Controllers
{
    public class BooksController : Controller
    {
        private readonly MVCBookAestheticContext _context;
        private readonly IWebHostEnvironment _environment;

        public BooksController(MVCBookAestheticContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Books
        public async Task<IActionResult> Index(string searchTitle, string searchAuthor, string searchGenre)
        {
            IQueryable<Book> booksQuery = _context.Book.Include(m => m.Author).Include(m => m.Genres).ThenInclude(m => m.Genre).Include(m => m.Reviews);
            IQueryable<string> genreQuery = _context.Genre.Select(g => g.GenreName).Distinct();

            if (!String.IsNullOrEmpty(searchTitle))
            {
                booksQuery = booksQuery.Where(b => b.Title.Contains(searchTitle));
            }
            if (!String.IsNullOrEmpty(searchAuthor))
            {
                booksQuery = booksQuery.Where(b => b.Author.FirstName.Contains(searchAuthor) || b.Author.LastName.Contains(searchAuthor));
            }
            if (!String.IsNullOrEmpty(searchGenre))
            {
                booksQuery = booksQuery.Where(s => s.Genres.Any(m => m.Genre.GenreName.Contains(searchGenre)));
            }

            var bookgenrevm = new BookGenreViewModel
            {
                Genres = new SelectList(await genreQuery.ToListAsync()),
                Books = await booksQuery.ToListAsync()
            };
            
            return View(bookgenrevm);
        }
        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .Include( b => b.Genres).ThenInclude(b => b.Genre).Include(b => b.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }


        // GET: Books/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName");
            var genres = _context.Genre.AsEnumerable();
            BookGenreCreateViewModel viewmodel = new BookGenreCreateViewModel
            {
                genresList = new MultiSelectList(genres, "Id", "GenreName")
            };
            return View(viewmodel);
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(BookGenreEditViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                if (viewmodel.Book.FrontPageFile != null && viewmodel.Book.FrontPageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewmodel.Book.FrontPageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewmodel.Book.FrontPageFile.CopyToAsync(fileStream);
                    }

                    // Save file path in the database
                    viewmodel.Book.FrontPage = "/uploads/" + uniqueFileName;
                    
                }
                if (viewmodel.Book.PdfFile != null && viewmodel.Book.PdfFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    string uniquePdfFileName = Guid.NewGuid().ToString() + "_" + viewmodel.Book.PdfFile.FileName;
                    string pdfFilePath = Path.Combine(uploadsFolder, uniquePdfFileName);

                    using (var pdfFileStream = new FileStream(pdfFilePath, FileMode.Create))
                    {
                        await viewmodel.Book.PdfFile.CopyToAsync(pdfFileStream);
                    }

                    // Save PDF file path in the database
                    viewmodel.Book.DownloadUrl = "/uploads/" + uniquePdfFileName;
                    
                }

                // Save your model to the database
                _context.Add(viewmodel.Book);
                await _context.SaveChangesAsync();
                foreach (int item in viewmodel.SelectedGenres)
                {
                    _context.BookGenre.Add(new BookGenre { GenreId = item, BookId = viewmodel.Book.Id });
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewmodel.Book);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var book = _context.Book.Where(m => m.Id == id).Include(m => m.Genres).First();
            
            if (book == null)
            {
                return NotFound();
            }
            var genres = _context.Genre.AsEnumerable();
            genres = genres.OrderBy(s => s.GenreName);

            BookGenreEditViewModel viewmodel = new BookGenreEditViewModel
            {
                Book = book,
                GenreList = new MultiSelectList(genres, "Id", "GenreName"),
                SelectedGenres = book.Genres.Select(sa => sa.GenreId)
            };

            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", book.AuthorId);
            return View(viewmodel);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, BookGenreEditViewModel viewmodel)

        {
            if (id != viewmodel.Book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewmodel.Book.FrontPageFile != null && viewmodel.Book.FrontPageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewmodel.Book.FrontPageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await viewmodel.Book.FrontPageFile.CopyToAsync(fileStream);
                        }

                        // Save file path in the database
                        viewmodel.Book.FrontPage = "/uploads/" + uniqueFileName;
                        
                    }
                    if (viewmodel.Book.PdfFile != null && viewmodel.Book.PdfFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                        string uniquePdfFileName = Guid.NewGuid().ToString() + "_" + viewmodel.Book.PdfFile.FileName;
                        string pdfFilePath = Path.Combine(uploadsFolder, uniquePdfFileName);

                        using (var pdfFileStream = new FileStream(pdfFilePath, FileMode.Create))
                        {
                            await viewmodel.Book.PdfFile.CopyToAsync(pdfFileStream);
                        }

                        // Save PDF file path in the database
                        viewmodel.Book.DownloadUrl = "/uploads/" + uniquePdfFileName;
                       
                    }
                    _context.Update(viewmodel.Book);
                    await _context.SaveChangesAsync(); //editirame book
                    IEnumerable<int> newGenreList = viewmodel.SelectedGenres; //i book genre tabelata
                    IEnumerable<int> prevGenreList = _context.BookGenre.Where(s => s.BookId == id).Select(s => s.GenreId);
                    IQueryable<BookGenre> toBeRemoved = _context.BookGenre.Where(s => s.BookId == id);
                    if (newGenreList != null)
                    {
                        toBeRemoved = toBeRemoved.Where(s => !newGenreList.Contains(s.GenreId));
                        foreach (int genreId in newGenreList)
                        {
                            if (!prevGenreList.Any(s => s == genreId))
                            {
                                _context.BookGenre.Add(new BookGenre { GenreId = genreId, BookId = id });
                            }
                        }
                    }
                    _context.BookGenre.RemoveRange(toBeRemoved);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(viewmodel.Book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", viewmodel.Book.AuthorId);
            return View(viewmodel);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
               .Include(b => b.Author)
               .Include(b => b.Genres).ThenInclude(b => b.Genre)
               .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
