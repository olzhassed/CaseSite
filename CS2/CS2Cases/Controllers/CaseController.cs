using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CS2Cases.Data;
using CS2Cases.Models;

namespace CS2Cases.Controllers;

public class CaseController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILogger<CaseController> _logger;

    public CaseController(AppDbContext db, ILogger<CaseController> logger)
    {
        _db = db;
        _logger = logger;
    }


    public async Task<IActionResult> Index()
    {
        var cases = await _db.Cases.Include(c => c.Skins).ToListAsync();
        return View(cases);
    }


    public async Task<IActionResult> Open(int id)
    {
        var caseItem = await _db.Cases
            .Include(c => c.Skins)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (caseItem == null) return NotFound();
        return View(caseItem);
    }


    [HttpPost]
    public async Task<IActionResult> Roll(int caseId)
    {
        var skins = await _db.Skins
            .Where(s => s.CaseId == caseId)
            .ToListAsync();

        if (!skins.Any())
            return Json(new { success = false, message = "Нет скинов в кейсе" });

        var result = RollSkin(skins);
        var condition = GetRandomCondition();


        var sessionId = HttpContext.Session.GetString("UserId") ?? Guid.NewGuid().ToString();
        HttpContext.Session.SetString("UserId", sessionId);

        _db.Inventory.Add(new UserInventory
        {
            UserId = sessionId,
            SkinId = result.Id,
            Condition = condition,
            ObtainedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        return Json(new
        {
            success = true,
            skinId = result.Id,
            skinName = result.Name,
            weaponType = result.WeaponType,
            rarity = result.Rarity,
            condition = condition,
            imageUrl = result.ImageUrl
        });
    }


    public async Task<IActionResult> Inventory()
    {
        var sessionId = HttpContext.Session.GetString("UserId");
        if (sessionId == null) return View(new List<UserInventory>());

        var inventory = await _db.Inventory
            .Where(i => i.UserId == sessionId)
            .Include(i => i.Skin)
            .OrderByDescending(i => i.ObtainedAt)
            .ToListAsync();

        return View(inventory);
    }

    private static Skin RollSkin(List<Skin> skins)
    {
        var total = skins.Sum(s => s.DropChance);
        var roll = (float)(new Random().NextDouble() * total);
        float cumulative = 0;

        foreach (var skin in skins.OrderBy(s => s.DropChance))
        {
            cumulative += skin.DropChance;
            if (roll <= cumulative) return skin;
        }
        return skins.Last();
    }

    private static string GetRandomCondition()
    {
        var conditions = new[] { "FN", "MW", "FT", "WW", "BS" };
        var weights = new[] { 0.03f, 0.24f, 0.33f, 0.24f, 0.16f };
        var roll = (float)new Random().NextDouble();
        float cum = 0;
        for (int i = 0; i < conditions.Length; i++)
        {
            cum += weights[i];
            if (roll <= cum) return conditions[i];
        }
        return "FT";
    }
}
