using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace CS2Cases.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WeaponType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rarity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DropChance = table.Column<float>(type: "real", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skins_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserInventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    SkinId = table.Column<int>(type: "int", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ObtainedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInventory_Skins_SkinId",
                        column: x => x.SkinId,
                        principalTable: "Skins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cases",
                columns: new[] { "Id", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Откройте и получите один из 17 скинов общества.", "/images/cases/prisma2.png", "Prisma 2 Case", 2.49m },
                    { 2, "Секретный агент. Совершенный удар.", "/images/cases/fracture.png", "Fracture Case", 1.99m },
                    { 3, "Яркие скины нового поколения.", "/images/cases/chroma3.png", "Chroma 3 Case", 3.49m }
                });

            migrationBuilder.InsertData(
                table: "Skins",
                columns: new[] { "Id", "CaseId", "DropChance", "ImageUrl", "Name", "Rarity", "WeaponType" },
                values: new object[,]
                {
                    { 1, 1, 3.2f, "/images/skins/ak47_phantom.png", "AK-47 | Phantom Disruptor", "Classified", "Rifle" },
                    { 2, 1, 3.2f, "/images/skins/m4a1s_playertwo.png", "M4A1-S | Player Two", "Classified", "Rifle" },
                    { 3, 1, 0.64f, "/images/skins/awp_wildfire.png", "AWP | Wildfire", "Covert", "Sniper" },
                    { 4, 1, 7.9f, "/images/skins/glock_neonoir.png", "Glock-18 | Neo-Noir", "Restricted", "Pistol" },
                    { 5, 1, 0.64f, "/images/skins/deagle_codered.png", "Desert Eagle | Code Red", "Covert", "Pistol" },
                    { 6, 1, 3.2f, "/images/skins/usps_cortex.png", "USP-S | Cortex", "Classified", "Pistol" },
                    { 7, 1, 15.98f, "/images/skins/p250_inferno.png", "P250 | Inferno", "MilSpec", "Pistol" },
                    { 8, 1, 15.98f, "/images/skins/mp9_hydra.png", "MP9 | Hydra", "MilSpec", "SMG" },
                    { 9, 1, 0.26f, "/images/skins/karambit_doppler.png", "★ Karambit | Doppler", "Gold", "Knife" },
                    { 10, 2, 0.64f, "/images/skins/ak47_ratrod.png", "AK-47 | Rat Rod", "Covert", "Rifle" },
                    { 11, 2, 0.64f, "/images/skins/m4a4_toothfairy.png", "M4A4 | Tooth Fairy", "Covert", "Rifle" },
                    { 12, 2, 3.2f, "/images/skins/deagle_kumicho.png", "Desert Eagle | Kumicho Dragon", "Classified", "Pistol" },
                    { 13, 2, 7.9f, "/images/skins/glock_vogue.png", "Glock-18 | Vogue", "Restricted", "Pistol" },
                    { 14, 2, 7.9f, "/images/skins/p90_cocoa.png", "P90 | Cocoa Rampage", "Restricted", "SMG" },
                    { 15, 2, 0.26f, "/images/skins/butterfly_fade.png", "★ Butterfly Knife | Fade", "Gold", "Knife" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skins_CaseId",
                table: "Skins",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInventory_SkinId",
                table: "UserInventory",
                column: "SkinId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInventory");

            migrationBuilder.DropTable(
                name: "Skins");

            migrationBuilder.DropTable(
                name: "Cases");
        }
    }
}
