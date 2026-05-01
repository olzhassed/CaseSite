using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CS2Cases.Data;
using CS2Cases.Models;
using System.Security.Cryptography;
using System.Text;

namespace CS2Cases.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;

    public AccountController(AppDbContext db) => _db = db;

    private string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password + "cs2salt"));
        return Convert.ToHexString(bytes);
    }

    private void SignIn(AppUser user)
    {
        HttpContext.Session.SetInt32("AuthUserId", user.Id);
        HttpContext.Session.SetString("AuthUsername", user.Username);
    }

    public AppUser? GetCurrentUser()
    {
        var id = HttpContext.Session.GetInt32("AuthUserId");
        return id == null ? null : _db.Users.Find(id);
    }

    public IActionResult Register()
    {
        if (HttpContext.Session.GetInt32("AuthUserId") != null)
            return RedirectToAction("Index", "Profile");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string username, string email, string password, string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Заполните все поля";
            return View();
        }
        if (password != confirmPassword)
        {
            ViewBag.Error = "Пароли не совпадают";
            return View();
        }
        if (password.Length < 6)
        {
            ViewBag.Error = "Пароль минимум 6 символов";
            return View();
        }
        if (await _db.Users.AnyAsync(u => u.Email == email))
        {
            ViewBag.Error = "Email уже используется";
            return View();
        }
        if (await _db.Users.AnyAsync(u => u.Username == username))
        {
            ViewBag.Error = "Имя пользователя занято";
            return View();
        }

        var user = new AppUser
        {
            Username = username.Trim(),
            Email = email.Trim().ToLower(),
            PasswordHash = HashPassword(password),
            Balance = 1000m,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        SignIn(user);

        return RedirectToAction("Index", "Profile");
    }

    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("AuthUserId") != null)
            return RedirectToAction("Index", "Profile");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Заполните все поля";
            return View();
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email.Trim().ToLower());
        if (user == null || user.PasswordHash != HashPassword(password))
        {
            ViewBag.Error = "Неверный email или пароль";
            return View();
        }

        SignIn(user);
        return RedirectToAction("Index", "Profile");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("AuthUserId");
        HttpContext.Session.Remove("AuthUsername");
        return RedirectToAction("Login");
    }
}