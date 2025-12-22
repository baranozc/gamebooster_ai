using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameBooster.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConstraints : Migration
    {
 protected override void Up(MigrationBuilder migrationBuilder)
{
    // ==========================================
    // TÜR 3: CHECK CONSTRAINTS
    // ==========================================

    // 1. Kısıt: RAM kontrolü
    migrationBuilder.Sql("ALTER TABLE Games ADD CONSTRAINT CK_Games_MinRam CHECK (MinRam > 0);");

    // ⚠️ ÇÖZÜM BURADA: Önce hatalı verileri (100'den büyükleri) 100'e çekiyoruz!
    // Böylece kural eklendiğinde hata vermeyecek.
    migrationBuilder.Sql("UPDATE Games SET DemandScore = 100 WHERE DemandScore > 100;");
    migrationBuilder.Sql("UPDATE Games SET DemandScore = 0 WHERE DemandScore < 0;");

// BUNUNLA DEĞİŞTİR:
migrationBuilder.Sql("ALTER TABLE Games ADD CONSTRAINT CK_Games_DemandScore CHECK (DemandScore >= 0);");

    // 3. Kısıt: GPU Vram kontrolü
    migrationBuilder.Sql("ALTER TABLE GPUs ADD CONSTRAINT CK_GPUs_Vram CHECK (Vram > 0);");


// ==========================================
    // TÜR 4: UNIQUE CONSTRAINTS (Benzersizlik Kısıtları)
    // ==========================================

    // DÜZELTME: Name(255) diyerek MySQL'e uzunluk sınırı veriyoruz.
    migrationBuilder.Sql("ALTER TABLE Games ADD CONSTRAINT UQ_Games_Name UNIQUE (Name(255));");

    // Aynı işlemi CPU için de yapıyoruz
    migrationBuilder.Sql("ALTER TABLE CPUs ADD CONSTRAINT UQ_CPUs_Name UNIQUE (Name(255));");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    // Geri alma (Rollback) işlemleri
    migrationBuilder.Sql("ALTER TABLE Games DROP CONSTRAINT CK_Games_MinRam;");
    migrationBuilder.Sql("ALTER TABLE Games DROP CONSTRAINT CK_Games_DemandScore;");
    migrationBuilder.Sql("ALTER TABLE GPUs DROP CONSTRAINT CK_GPUs_Vram;");
    
    // MySQL'de Unique Constraint genellikle Index olarak tutulur, silerken index adıyla silmek gerekebilir.
    migrationBuilder.Sql("ALTER TABLE Games DROP INDEX UQ_Games_Name;");
    migrationBuilder.Sql("ALTER TABLE CPUs DROP INDEX UQ_CPUs_Name;");
}
    }
}
