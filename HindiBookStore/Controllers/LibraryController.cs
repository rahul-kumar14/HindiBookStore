using HindiBookStore.Data;
using HindiBookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HindiBookStore.Controllers
{
    public class LibraryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LibraryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Library
        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            var books = _context.Books.Include(b => b.Category).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                books = books.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
            }

            if (categoryId.HasValue)
            {
                books = books.Where(b => b.CategoryId == categoryId);
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategoryId = categoryId;

            return View(await books.ToListAsync());
        }

        // GET: Library/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Library/Read/5
        // Redirects to ViewPdf to open PDF directly in browser
        public async Task<IActionResult> Read(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (book == null || string.IsNullOrEmpty(book.PdfFilePath))
            {
                return NotFound();
            }

            // Redirect to ViewPdf to open in browser tab
            return RedirectToAction(nameof(ViewPdf), new { id = book.Id });
        }

        // GET: Library/ViewPdf/5
        public async Task<IActionResult> ViewPdf(int id)
        {
            var book = await _context.Books.FindAsync(id);
            
            if (book == null || string.IsNullOrEmpty(book.PdfFilePath))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.PdfFilePath.TrimStart('/'));
            
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            
            // Return PDF for inline viewing (not download)
            return File(memory, "application/pdf");
        }

        // GET: Library/Download/5
        public async Task<IActionResult> Download(int id)
        {
            var book = await _context.Books.FindAsync(id);
            
            if (book == null || string.IsNullOrEmpty(book.PdfFilePath))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.PdfFilePath.TrimStart('/'));
            
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            
            // Sanitize filename for download - ensure .pdf extension
            var sanitizedTitle = book.Title.Replace(" ", "_").Replace("/", "_");
            var fileName = sanitizedTitle.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) 
                ? sanitizedTitle 
                : $"{sanitizedTitle}.pdf";
            
            return File(memory, "application/pdf", fileName);
        }
    }
}
