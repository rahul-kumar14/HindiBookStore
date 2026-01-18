using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HindiBookStore.Models;
using HindiBookStore.Data;
using Microsoft.EntityFrameworkCore;

namespace HindiBookStore.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var featuredBooks = await _context.Books
            .Include(b => b.Category)
            .OrderByDescending(b => b.CreatedAt)
            .Take(8)
            .ToListAsync();
        return View(featuredBooks);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

