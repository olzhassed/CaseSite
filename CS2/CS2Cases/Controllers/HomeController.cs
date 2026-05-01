using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CS2Cases.Data;

namespace CS2Cases.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var cases = await _db.Cases.Include(c => c.Skins).Take(3).ToListAsync();
        return View(cases);
    }
}
