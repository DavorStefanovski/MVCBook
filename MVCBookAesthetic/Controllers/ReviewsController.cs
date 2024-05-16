using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCBookAesthetic.Areas.Identity.Data;
using MVCBookAesthetic.Data;
using MVCBookAesthetic.Models;
using MVCBookAesthetic.ViewModels;
namespace MVCBookAesthetic.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly MVCBookAestheticContext _context;
        private readonly UserManager<MVCBookAestheticUser> _userManager;
        public ReviewsController(MVCBookAestheticContext context, UserManager<MVCBookAestheticUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var mVCBookAestheticContext = _context.Review.Include(r => r.Book);
            return View(await mVCBookAestheticContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

      
        // GET: Reviews/Create
        public async Task<IActionResult> Create(int id)
        {
            var book = await _context.Book.Where(s => s.Id == id).SingleOrDefaultAsync();
            if (book == null)
            {
                return NotFound();
            }

            var bookReview = new BookReview
            {
                Title = book.Title,
                Id = book.Id,
                Review = new Review()
            };

            return View(bookReview);
        }


        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookReview viewModel)
        {
            //if (ModelState.IsValid)
            
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var book = await _context.Book.FindAsync(viewModel.Id);
                if (book == null)
                {
                    return NotFound("Book not found");
                }

                var newEntry = new Review
                {
                    Comment = viewModel.Review.Comment,
                    Rating = viewModel.Review.Rating,
                    AppUser = user.Email,
                    BookId = book.Id
                };

                _context.Review.Add(newEntry); // Use the correct DbSet to add the new entry
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            

            //return View(viewModel);
        }



        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Title", review.BookId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,AppUser,Comment,Rating")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
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
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Title", review.BookId);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Review.FindAsync(id);
            if (review != null)
            {
                _context.Review.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Review.Any(e => e.Id == id);
        }
    }
}
