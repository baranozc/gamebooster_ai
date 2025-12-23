using GameBooster.Core.Entities;
using GameBooster.Core.Interfaces;
using GameBooster.Data;
using GameBooster.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoapCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Önce bağlantı cümlesini alıyoruz
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// MSSQL yerine MySQL (Pomelo) kullanıyoruz
builder.Services.AddDbContext<GameBoosterDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);
// ----------------------------------------------

// Identity Ayarlarımız (Aynen Korundu)
builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false; 
    options.Password.RequiredLength = 3; 

    options.User.RequireUniqueEmail = true; 
})
.AddEntityFrameworkStores<GameBoosterDbContext>()
.AddDefaultTokenProviders();

// Servislerin Eklenmesi
//SOAP servisimizi ekledik.
builder.Services.AddHttpClient<GameBooster.Service.Services.ISoapService, GameBooster.Service.Services.SoapService>();

builder.Services.AddScoped<IHardwareService, HardwareService>();

builder.Services.AddSoapCore(); 
builder.Services.AddScoped<ISystemRequirementService, SystemRequirementService>();

builder.Services.AddControllersWithViews();
builder.Services.AddGrpcClient<GameBooster.Grpc.BottleneckCalculator.BottleneckCalculatorClient>(o =>
{
    o.Address = new Uri("http://localhost:5142");
});

var app = builder.Build();

// Veritabanı ve Seed İşlemleri (Aynen Korundu)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<GameBoosterDbContext>(); 

    // Veritabanı yoksa oluşturur (Migration yerine hızlı çözüm için açıksa)
    // Eğer Migration kullanacaksan burayı kapatıp 'dotnet ef database update' yapman gerekebilir.
    // Şimdilik senin koduna sadık kalarak açık bırakıyorum.
    context.Database.EnsureCreated();

    await DbSeeder.SeedAsync(context);
    await DbSeeder.SeedRolesAndAdminAsync(services);
}

// HTTP Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/500");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/PageNotFound");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Kimlik Doğrulama
app.UseAuthorization();  // Yetkilendirme

// SOAP Endpoint
((IApplicationBuilder)app).UseSoapEndpoint<ISystemRequirementService>(
    "/Service.asmx",
    new SoapEncoderOptions(),
    SoapSerializer.DataContractSerializer,
    caseInsensitivePath: true
);

// Not: Senin kodunda UseAuthorization iki kere vardı, gereksiz olanı sildim.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();