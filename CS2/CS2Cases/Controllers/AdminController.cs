using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CS2Cases.Data;
using CS2Cases.Models;

namespace CS2Cases.Controllers;

public class AdminController : Controller
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db) => _db = db;

    private bool IsAdmin() => HttpContext.Session.GetString("AdminAuth") == "true";

    public IActionResult Index()
    {
        if (!IsAdmin()) return RedirectToAction("Login");
        return RedirectToAction("Dashboard");
    }

    public IActionResult Login()
    {
        if (IsAdmin()) return RedirectToAction("Dashboard");
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        if (username == "admin" && password == "admin")
        {
            HttpContext.Session.SetString("AdminAuth", "true");
            return RedirectToAction("Dashboard");
        }
        ViewBag.Error = "Неверный логин или пароль";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("AdminAuth");
        return RedirectToAction("Login");
    }

    public async Task<IActionResult> Dashboard()
    {
        if (!IsAdmin()) return RedirectToAction("Login");

        ViewBag.CasesCount = await _db.Cases.CountAsync();
        ViewBag.SkinsCount = await _db.Skins.CountAsync();
        ViewBag.InventoryCount = await _db.Inventory.CountAsync();
        return View();
    }

    public async Task<IActionResult> Cases()
    {
        if (!IsAdmin()) return RedirectToAction("Login");
        var cases = await _db.Cases.Include(c => c.Skins).ToListAsync();
        return View(cases);
    }

    public IActionResult CreateCase()
    {
        if (!IsAdmin()) return RedirectToAction("Login");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCase(Case model)
    {
        if (!IsAdmin()) return RedirectToAction("Login");
        if (!ModelState.IsValid) return View(model);

        _db.Cases.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Кейс «{model.Name}» успешно добавлен!";
        return RedirectToAction("Cases");
    }

    public async Task<IActionResult> EditCase(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login");
        var c = await _db.Cases.FindAsync(id);
        if (c == null) return NotFound();
        return View(c);
    }

    [HttpPost]
    public async Task<IActionResult> EditCase(Case model)
    {
        if (!IsAdmin()) return RedirectToAction("Login");
        if (!ModelState.IsValid) return View(model);

        _db.Cases.Update(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Кейс «{model.Name}» обновлён!";
        return RedirectToAction("Cases");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCase(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login");
        var c = await _db.Cases.FindAsync(id);
        if (c != null)
        {
            _db.Cases.Remove(c);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Кейс «{c.Name}» удалён.";
        }
        return RedirectToAction("Cases");
    }

    public async Task<IActionResult> Skins()
    {
        if (!IsAdmin()) return RedirectToAction("Login");
        var skins = await _db.Skins.Include(s => s.Case).ToListAsync();
        return View(skins);
    }

    public async Task<IActionResult> CreateSkin()
    {
        if (!IsAdmin()) return RedirectToAction("Login");
        ViewBag.Cases = await _db.Cases.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateSkin(Skin model)
    {
        if (!IsAdmin()) return RedirectToAction("Login");
        if (!ModelState.IsValid)
        {
            ViewBag.Cases = await _db.Cases.ToListAsync();
            return View(model);
        }

        _db.Skins.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Скин «{model.Name}» успешно добавлен!";
        return RedirectToAction("Skins");
    }

    public async Task<IActionResult> EditSkin(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login");
        var s = await _db.Skins.FindAsync(id);
        if (s == null) return NotFound();
        ViewBag.Cases = await _db.Cases.ToListAsync();
        return View(s);
    }

    [HttpPost]
    public async Task<IActionResult> EditSkin(Skin model)
    {
        if (!IsAdmin()) return RedirectToAction("Login");
        if (!ModelState.IsValid)
        {
            ViewBag.Cases = await _db.Cases.ToListAsync();
            return View(model);
        }

        _db.Skins.Update(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Скин «{model.Name}» обновлён!";
        return RedirectToAction("Skins");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSkin(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login");
        var s = await _db.Skins.FindAsync(id);
        if (s != null)
        {
            _db.Skins.Remove(s);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Скин «{s.Name}» удалён.";
        }
        return RedirectToAction("Skins");
    }


    public async Task<IActionResult> Inventory()
    {
        if (!IsAdmin()) return RedirectToAction("Login");
        var inv = await _db.Inventory
            .Include(i => i.Skin)
            .OrderByDescending(i => i.ObtainedAt)
            .ToListAsync();
        return View(inv);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteInventory(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login");
        var item = await _db.Inventory.FindAsync(id);
        if (item != null)
        {
            _db.Inventory.Remove(item);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Запись инвентаря удалена.";
        }
        return RedirectToAction("Inventory");
    }
}
