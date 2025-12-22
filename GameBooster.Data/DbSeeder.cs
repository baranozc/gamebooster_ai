using GameBooster.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore; // Any() metodu için gerekli olabilir

namespace GameBooster.Data
{
    public class DbSeeder
    {
        // 1. METOT: DONANIM VE OYUN VERİLERİNİ YÜKLER
        public static async Task SeedAsync(GameBoosterDbContext context)
        {
            // --- 1. GPU VERİLERİ ---
            if (!context.GPUs.Any())
            {
                var gpus = new List<GPU>
                {
                    new GPU { Name = "GeForce RTX 5090", Vram = 32, CoreCount = 21760 },
                    new GPU { Name = "GeForce RTX 4090", Vram = 24, CoreCount = 16384 },
                    new GPU { Name = "GeForce RTX 5080 SUPER", Vram = 24, CoreCount = 10752 },
                    new GPU { Name = "Radeon RX 7900 XTX", Vram = 24, CoreCount = 6144 },
                    new GPU { Name = "GeForce RTX 3090 Ti", Vram = 24, CoreCount = 10752 },
                    new GPU { Name = "GeForce RTX 3090", Vram = 24, CoreCount = 10496 },
                    new GPU { Name = "Radeon RX 7900 XT", Vram = 20, CoreCount = 5376 },
                    new GPU { Name = "GeForce RTX 5080", Vram = 16, CoreCount = 10752 },
                    new GPU { Name = "GeForce RTX 4080 SUPER", Vram = 16, CoreCount = 10240 },
                    new GPU { Name = "GeForce RTX 4080", Vram = 16, CoreCount = 9728 },
                    new GPU { Name = "GeForce RTX 5070 Ti", Vram = 16, CoreCount = 8960 },
                    new GPU { Name = "GeForce RTX 4070 Ti SUPER", Vram = 16, CoreCount = 8448 },
                    new GPU { Name = "Radeon RX 6950 XT", Vram = 16, CoreCount = 5120 },
                    new GPU { Name = "Radeon RX 6900 XT", Vram = 16, CoreCount = 5120 },
                    new GPU { Name = "Radeon RX 7900 GRE", Vram = 16, CoreCount = 5120 },
                    new GPU { Name = "GeForce RTX 5060 Ti 16 GB", Vram = 16, CoreCount = 4608 },
                    new GPU { Name = "Radeon RX 6800 XT", Vram = 16, CoreCount = 4608 },
                    new GPU { Name = "Radeon RX 7800 XT", Vram = 16, CoreCount = 3840 },
                    new GPU { Name = "Radeon RX 6800", Vram = 16, CoreCount = 3840 },
                    new GPU { Name = "Radeon RX 9070 XT", Vram = 16, CoreCount = 4096 },
                    new GPU { Name = "GeForce RTX 3080 Ti", Vram = 12, CoreCount = 10240 },
                    new GPU { Name = "GeForce RTX 4070 Ti", Vram = 12, CoreCount = 7680 },
                    new GPU { Name = "GeForce RTX 3080 12GB", Vram = 12, CoreCount = 8960 },
                    new GPU { Name = "Radeon RX 7700 XT", Vram = 12, CoreCount = 3456 },
                    new GPU { Name = "GeForce RTX 4070", Vram = 12, CoreCount = 5888 },
                };
                await context.GPUs.AddRangeAsync(gpus);
            }

            // --- 2. CPU VERİLERİ ---
            if (!context.CPUs.Any())
            {
                var cpus = new List<CPU>
                {
                    new CPU { Name = "Ryzen 9 9950X3D", CoreCount = 16, ThreadCount = 32, MaxFrequency = 5.7d, Cache = 128 },
                    new CPU { Name = "Ryzen 7 9800X3D", CoreCount = 8, ThreadCount = 16, MaxFrequency = 5.2d, Cache = 96 },
                    new CPU { Name = "Ryzen 7 7800X3D", CoreCount = 8, ThreadCount = 16, MaxFrequency = 5.0d, Cache = 96 },
                    new CPU { Name = "Core Ultra 9 285K", CoreCount = 24, ThreadCount = 24, MaxFrequency = 5.7d, Cache = 36 },
                    new CPU { Name = "Ryzen 9 9950X", CoreCount = 16, ThreadCount = 32, MaxFrequency = 5.7d, Cache = 64 },
                    new CPU { Name = "Ryzen 9 9900X", CoreCount = 12, ThreadCount = 24, MaxFrequency = 5.6d, Cache = 64 },
                    new CPU { Name = "Core Ultra 7 265K", CoreCount = 20, ThreadCount = 20, MaxFrequency = 5.5d, Cache = 30 },
                    new CPU { Name = "Ryzen 7 9700X", CoreCount = 8, ThreadCount = 16, MaxFrequency = 5.5d, Cache = 32 },
                    new CPU { Name = "Ryzen 5 9600X", CoreCount = 6, ThreadCount = 12, MaxFrequency = 5.4d, Cache = 32 },
                    new CPU { Name = "Ryzen 7 7700X", CoreCount = 8, ThreadCount = 16, MaxFrequency = 5.4d, Cache = 32 },
                    new CPU { Name = "Ryzen 5 7600X", CoreCount = 6, ThreadCount = 12, MaxFrequency = 5.3d, Cache = 32 },
                    new CPU { Name = "Core Ultra 9 275HX", CoreCount = 24, ThreadCount = 24, MaxFrequency = 5.4d, Cache = 36 },
                    new CPU { Name = "Ryzen 9 7950X3D", CoreCount = 16, ThreadCount = 32, MaxFrequency = 5.7d, Cache = 128 },
                    new CPU { Name = "Ryzen 7 5800X3D", CoreCount = 8, ThreadCount = 16, MaxFrequency = 4.5d, Cache = 96 },
                    new CPU { Name = "Ryzen 7 8845HS", CoreCount = 8, ThreadCount = 16, MaxFrequency = 5.1d, Cache = 16 },
                    new CPU { Name = "Ryzen 7 8700G", CoreCount = 8, ThreadCount = 16, MaxFrequency = 5.1d, Cache = 16 },
                    new CPU { Name = "Ryzen 5 8600G", CoreCount = 6, ThreadCount = 12, MaxFrequency = 5.0d, Cache = 16 },
                    new CPU { Name = "Ryzen 7 8700F", CoreCount = 8, ThreadCount = 16, MaxFrequency = 5.0d, Cache = 16 },
                    new CPU { Name = "Ryzen 5 8500G", CoreCount = 6, ThreadCount = 12, MaxFrequency = 5.0d, Cache = 16 },
                    new CPU { Name = "Ryzen AI 9 HX 370", CoreCount = 12, ThreadCount = 24, MaxFrequency = 5.1d, Cache = 16 },
                    new CPU { Name = "Ryzen Threadripper PRO 9995WX", CoreCount = 96, ThreadCount = 192, MaxFrequency = 5.4d, Cache = 384 },
                    new CPU { Name = "Ryzen AI Max+ 395", CoreCount = 16, ThreadCount = 32, MaxFrequency = 5.1d, Cache = 64 },
                    new CPU { Name = "Ryzen 5 8400F", CoreCount = 6, ThreadCount = 12, MaxFrequency = 4.7d, Cache = 16 },
                    new CPU { Name = "Ryzen 7 260", CoreCount = 8, ThreadCount = 16, MaxFrequency = 5.1d, Cache = 16 },
                    new CPU { Name = "Ryzen AI 7 350", CoreCount = 8, ThreadCount = 16, MaxFrequency = 5.0d, Cache = 8 },
                };
                await context.CPUs.AddRangeAsync(cpus);
            }

            // --- 3. OYUN VERİLERİ ---
            if (!context.Games.Any())
            {
                var games = new List<Game>
                {
                    new Game { Name = "Black Myth Wukong", DemandScore = 182.15f, MinRam = 16, MinStorage = 130, Description = "Aksiyon RPG.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Assassin's Creed Shadows", DemandScore = 163.4f, MinRam = 16, MinStorage = 100, Description = "Tarihi aksiyon.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Monster Hunter Wilds", DemandScore = 153.37f, MinRam = 16, MinStorage = 90, Description = "Canavar avı.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Avowed", DemandScore = 146.63f, MinRam = 16, MinStorage = 80, Description = "Fantezi RPG.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Star Wars Outlaws", DemandScore = 117.23f, MinRam = 16, MinStorage = 65, Description = "Uzay macerası.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Stalker 2", DemandScore = 113.12f, MinRam = 16, MinStorage = 150, Description = "Kıyamet sonrası.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Starfield", DemandScore = 102.88f, MinRam = 16, MinStorage = 125, Description = "Uzay RPG.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "The Last of Us Part 1", DemandScore = 97.85f, MinRam = 16, MinStorage = 100, Description = "Hayatta kalma.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Alan Wake 2", DemandScore = 97.09f, MinRam = 16, MinStorage = 90, Description = "Korku gerilim.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Dragon Age Veilguard", DemandScore = 96.81f, MinRam = 16, MinStorage = 100, Description = "RPG efsanesi.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Hogwarts Legacy", DemandScore = 84.53f, MinRam = 16, MinStorage = 85, Description = "Büyücülük dünyası.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Horizon Forbidden West", DemandScore = 79.11f, MinRam = 16, MinStorage = 150, Description = "Açık dünya.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "God of War Ragnarök", DemandScore = 76.16f, MinRam = 16, MinStorage = 190, Description = "İskandinav mitolojisi.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Ghost Of Tsushima", DemandScore = 75.99f, MinRam = 16, MinStorage = 60, Description = "Samuray destanı.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Spider-Man 2", DemandScore = 74.74f, MinRam = 16, MinStorage = 90, Description = "Süper kahraman.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Kingdom Come 2", DemandScore = 69.49f, MinRam = 16, MinStorage = 80, Description = "Orta çağ RPG.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Cyberpunk 2077", DemandScore = 62.46f, MinRam = 12, MinStorage = 70, Description = "Gelecek temalı.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Baldur's Gate 3", DemandScore = 56.63f, MinRam = 16, MinStorage = 150, Description = "D&D macera.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "No Rest for the Wicked", DemandScore = 53.42f, MinRam = 16, MinStorage = 40, Description = "Karanlık aksiyon.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Elden Ring", DemandScore = 52.19f, MinRam = 12, MinStorage = 60, Description = "Zorlu aksiyon.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "F1 24", DemandScore = 45.66f, MinRam = 16, MinStorage = 50, Description = "Yarış simülasyonu.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "DOOM Eternal", DemandScore = 42.72f, MinRam = 8, MinStorage = 50, Description = "Vur kır parçala.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "The Witcher 3:Wild Hunt", DemandScore = 34.34f, MinRam = 8, MinStorage = 50, Description = "Canavar avcısı.", ImageUrl = "/images/games/default.jpg" },
                    new Game { Name = "Counter-Strike 2", DemandScore = 23.17f, MinRam = 8, MinStorage = 85, Description = "Takım tabanlı FPS.", ImageUrl = "/images/games/default.jpg" },
                };
                await context.Games.AddRangeAsync(games);
            }

            await context.SaveChangesAsync();
        }

        // 2. METOT: ADMİN VE ROLLERİ YÜKLER
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            // 1. ROLLERİ OLUŞTUR
            string[] roleNames = { "Admin", "Member" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }

            // 2. ADMIN KULLANCISINI OLUŞTUR
            var adminEmail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    CreatedDate = DateTime.Now
                };

                var createPowerUser = await userManager.CreateAsync(newAdmin, "Admin123");
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}