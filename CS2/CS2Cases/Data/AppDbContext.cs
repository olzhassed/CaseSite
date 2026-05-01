using Microsoft.EntityFrameworkCore;
using CS2Cases.Models;

namespace CS2Cases.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Case> Cases => Set<Case>();
    public DbSet<Skin> Skins => Set<Skin>();
    public DbSet<UserInventory> Inventory => Set<UserInventory>();
    public DbSet<UserProfile> Profiles => Set<UserProfile>();
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Case>(entity =>
        {
            entity.ToTable("Cases");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Skin>(entity =>
        {
            entity.ToTable("Skins");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.WeaponType).HasMaxLength(100);
            entity.Property(e => e.Rarity).HasMaxLength(50);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.HasOne(e => e.Case)
                  .WithMany(c => c.Skins)
                  .HasForeignKey(e => e.CaseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserInventory>(entity =>
        {
            entity.ToTable("UserInventory");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.Condition).HasMaxLength(10);
            entity.HasOne(e => e.Skin)
                  .WithMany()
                  .HasForeignKey(e => e.SkinId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
       
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("UserProfiles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SessionId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.Username).HasMaxLength(100);
            entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalEarned).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.SessionId).IsUnique();
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("AppUsers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalEarned).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
        });

        modelBuilder.Entity<Case>().HasData(
            new Case { Id = 1, Name = "Prisma 2 Case", Description = "Откройте и получите один из 17 скинов общества.",
                ImageUrl = "/images/cases/prisma2.png", Price = 2.49m },
            new Case { Id = 2, Name = "Fracture Case", Description = "Секретный агент. Совершенный удар.",
                ImageUrl = "/images/cases/fracture.png", Price = 1.99m },
            new Case { Id = 3, Name = "Chroma 3 Case", Description = "Яркие скины нового поколения.",
                ImageUrl = "/images/cases/chroma3.png", Price = 3.49m }
        );

        modelBuilder.Entity<Skin>().HasData(
            new Skin { Id = 1, CaseId = 1, Name = "AK-47 | Phantom Disruptor", WeaponType = "Rifle",
                Rarity = "Classified", DropChance = 3.2f, ImageUrl = "/images/skins/ak47_phantom.png" },
            new Skin { Id = 2, CaseId = 1, Name = "M4A1-S | Player Two", WeaponType = "Rifle",
                Rarity = "Classified", DropChance = 3.2f, ImageUrl = "/images/skins/m4a1s_playertwo.png" },
            new Skin { Id = 3, CaseId = 1, Name = "AWP | Wildfire", WeaponType = "Sniper",
                Rarity = "Covert", DropChance = 0.64f, ImageUrl = "/images/skins/awp_wildfire.png" },
            new Skin { Id = 4, CaseId = 1, Name = "Glock-18 | Neo-Noir", WeaponType = "Pistol",
                Rarity = "Restricted", DropChance = 7.9f, ImageUrl = "/images/skins/glock_neonoir.png" },
            new Skin { Id = 5, CaseId = 1, Name = "Desert Eagle | Code Red", WeaponType = "Pistol",
                Rarity = "Covert", DropChance = 0.64f, ImageUrl = "/images/skins/deagle_codered.png" },
            new Skin { Id = 6, CaseId = 1, Name = "USP-S | Cortex", WeaponType = "Pistol",
                Rarity = "Classified", DropChance = 3.2f, ImageUrl = "/images/skins/usps_cortex.png" },
            new Skin { Id = 7, CaseId = 1, Name = "P250 | Inferno", WeaponType = "Pistol",
                Rarity = "MilSpec", DropChance = 15.98f, ImageUrl = "/images/skins/p250_inferno.png" },
            new Skin { Id = 8, CaseId = 1, Name = "MP9 | Hydra", WeaponType = "SMG",
                Rarity = "MilSpec", DropChance = 15.98f, ImageUrl = "/images/skins/mp9_hydra.png" },
            new Skin { Id = 9, CaseId = 1, Name = "★ Karambit | Doppler", WeaponType = "Knife",
                Rarity = "Gold", DropChance = 0.26f, ImageUrl = "/images/skins/karambit_doppler.png" }
        );

        modelBuilder.Entity<Skin>().HasData(
            new Skin { Id = 10, CaseId = 2, Name = "AK-47 | Rat Rod", WeaponType = "Rifle",
                Rarity = "Covert", DropChance = 0.64f, ImageUrl = "/images/skins/ak47_ratrod.png" },
            new Skin { Id = 11, CaseId = 2, Name = "M4A4 | Tooth Fairy", WeaponType = "Rifle",
                Rarity = "Covert", DropChance = 0.64f, ImageUrl = "/images/skins/m4a4_toothfairy.png" },
            new Skin { Id = 12, CaseId = 2, Name = "Desert Eagle | Kumicho Dragon", WeaponType = "Pistol",
                Rarity = "Classified", DropChance = 3.2f, ImageUrl = "/images/skins/deagle_kumicho.png" },
            new Skin { Id = 13, CaseId = 2, Name = "Glock-18 | Vogue", WeaponType = "Pistol",
                Rarity = "Restricted", DropChance = 7.9f, ImageUrl = "/images/skins/glock_vogue.png" },
            new Skin { Id = 14, CaseId = 2, Name = "P90 | Cocoa Rampage", WeaponType = "SMG",
                Rarity = "Restricted", DropChance = 7.9f, ImageUrl = "/images/skins/p90_cocoa.png" },
            new Skin { Id = 15, CaseId = 2, Name = "★ Butterfly Knife | Fade", WeaponType = "Knife",
                Rarity = "Gold", DropChance = 0.26f, ImageUrl = "/images/skins/butterfly_fade.png" }
        );
    }
}
