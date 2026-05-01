namespace CS2Cases.Models;

public class Case
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public decimal Price { get; set; }
    public List<Skin> Skins { get; set; } = new();
}

public class Skin
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string WeaponType { get; set; } = "";
    public string Rarity { get; set; } = "Common";
    public float DropChance { get; set; }
    public string ImageUrl { get; set; } = "";
    public decimal SellPrice { get; set; } = 0;
    public int CaseId { get; set; }
    public Case? Case { get; set; }
}

public class UserInventory
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public int SkinId { get; set; }
    public Skin? Skin { get; set; }
    public string Condition { get; set; } = "";
    public bool IsSold { get; set; } = false;
    public DateTime ObtainedAt { get; set; } = DateTime.UtcNow;
}

public class UserProfile
{
    public int Id { get; set; }
    public string SessionId { get; set; } = "";
    public string Username { get; set; } = "Игрок";
    public decimal Balance { get; set; } = 1000m;
    public int CasesOpened { get; set; } = 0;
    public int SkinsSold { get; set; } = 0;
    public decimal TotalEarned { get; set; } = 0m;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public decimal Balance { get; set; } = 1000m;
    public int CasesOpened { get; set; } = 0;
    public int SkinsSold { get; set; } = 0;
    public decimal TotalEarned { get; set; } = 0m;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}