using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameBooster.Data.Migrations
{
    /// <inheritdoc />
    public partial class FinalTouches : Migration
    {
        /// <inheritdoc />
protected override void Up(MigrationBuilder migrationBuilder)
{
    // ==========================================
    // 1. MASKELEME (Data Masking) - 10 PUAN
    // ==========================================
    // Önce eski maskesiz view'ı siliyoruz.
    migrationBuilder.Sql("DROP VIEW IF EXISTS vw_UserStats;");

    // Şimdi veriyi gizleyen (Maskeleyen) yeni halini oluşturuyoruz.
    // Örnek: 'ali@gmail.com' -> 'a***@gmail.com' şeklinde görünür.
    migrationBuilder.Sql(@"
        CREATE VIEW vw_UserStats AS
        SELECT 
            UserName, 
            CONCAT(LEFT(Email, 2), '****', SUBSTRING(Email, INSTR(Email, '@'))) AS MaskedEmail,
            CONCAT(LEFT(PhoneNumber, 3), '******', RIGHT(PhoneNumber, 2)) AS MaskedPhone
        FROM AspNetUsers;
    ");

    // ==========================================
    // 2. PERFORMANS (Indexing) - 10 PUAN
    // ==========================================
    // Oyunları en çok RAM ve Puana göre filtreliyoruz, o yüzden bunlara INDEX atıyoruz.
    // Bu işlem sorguların çok daha hızlı çalışmasını sağlar.
    
    migrationBuilder.Sql("CREATE INDEX IX_Games_MinRam ON Games(MinRam);");
    migrationBuilder.Sql("CREATE INDEX IX_Games_DemandScore ON Games(DemandScore);");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    // Geri alma işlemleri
    migrationBuilder.Sql("DROP INDEX IX_Games_MinRam ON Games;");
    migrationBuilder.Sql("DROP INDEX IX_Games_DemandScore ON Games;");
    migrationBuilder.Sql("DROP VIEW IF EXISTS vw_UserStats;");
    
    // Eski halini geri getirmek istersek (Basit hali)
    migrationBuilder.Sql(@"
        CREATE VIEW vw_UserStats AS
        SELECT UserName, Email, PhoneNumber 
        FROM AspNetUsers;
    ");
}
    }
}
