using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CS2Cases.Data;
using CS2Cases.Models;

namespace CS2Cases.Controllers;

public class ProfileController : Controller
{
    private readonly AppDbContext _db;

    public ProfileController(AppDbContext db) => _db = db;

    private string GetSessionId()
    {
        var id = HttpContext.Session.GetString("UserId") ?? Guid.NewGuid().ToString();
        HttpContext.Session.SetString("UserId", id);
        return id;
    }

    public async Task<UserProfile> GetOrCreateProfile(string sessionId)
    {
        var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.SessionId == sessionId);
        if (profile == null)
        {
            profile = new UserProfile { SessionId = sessionId };
            _db.Profiles.Add(profile);
            await _db.SaveChangesAsync();
        }
        return profile;
    }

    public async Task<IActionResult> Index()
    {
        var sessionId = GetSessionId();
        var profile = await GetOrCreateProfile(sessionId);
        var inventory = await _db.Inventory
            .Where(i => i.UserId == sessionId && !i.IsSold)
            .Include(i => i.Skin)
            .OrderByDescending(i => i.ObtainedAt)
            .ToListAsync();

        ViewBag.Inventory = inventory;
        return View(profile);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return RedirectToAction("Index");
        var sessionId = GetSessionId();
        var profile = await GetOrCreateProfile(sessionId);
        profile.Username = username.Trim().Length > 30 ? username.Trim()[..30] : username.Trim();
        await _db.SaveChangesAsync();
        TempData["Success"] = "Имя обновлено!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> SellSkin(int inventoryId)
    {
        var sessionId = GetSessionId();
        var item = await _db.Inventory
            .Include(i => i.Skin)
            .FirstOrDefaultAsync(i => i.Id == inventoryId && i.UserId == sessionId && !i.IsSold);

        if (item == null)
            return Json(new { success = false, message = "Предмет не найден" });

        var profile = await GetOrCreateProfile(sessionId);
        var price = item.Skin!.SellPrice;

        item.IsSold = true;
        profile.Balance += price;
        profile.SkinsSold++;
        profile.TotalEarned += price;

        await _db.SaveChangesAsync();

        return Json(new { success = true, earned = price, newBalance = profile.Balance, skinName = item.Skin.Name });
    }
}
