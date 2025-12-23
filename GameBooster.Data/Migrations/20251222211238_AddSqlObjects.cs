using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameBooster.Data.Migrations
{
    public partial class AddSqlObjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
{
    // VIEWS
    migrationBuilder.Sql(@"CREATE VIEW vw_CpuCacheReport AS SELECT Name, Cache, ThreadCount FROM CPUs;");
    migrationBuilder.Sql(@"CREATE VIEW vw_GpuPerformance AS SELECT Name, Vram, CoreCount FROM GPUs;");
    migrationBuilder.Sql(@"CREATE VIEW vw_HighEndSystems AS SELECT us.SystemName, gpu.Name as GpuName, cpu.Name as CpuName FROM UserSystems us JOIN GPUs gpu ON us.GPUId = gpu.Id JOIN CPUs cpu ON us.CPUId = cpu.Id WHERE gpu.Vram >= 8;");
    migrationBuilder.Sql(@"CREATE VIEW vw_SystemDetails AS SELECT us.Id, us.SystemName, u.UserName, us.RamAmount FROM UserSystems us JOIN AspNetUsers u ON us.AppUserId = u.Id;");
    migrationBuilder.Sql(@"CREATE VIEW vw_UserStats AS SELECT UserName, Email, PhoneNumber FROM AspNetUsers;");

    // PROCEDURES
    migrationBuilder.Sql(@"CREATE PROCEDURE sp_CountUserSystems(IN userId INT, OUT total INT) BEGIN SELECT COUNT(*) INTO total FROM UserSystems WHERE AppUserId = userId; END;");
    migrationBuilder.Sql(@"CREATE PROCEDURE sp_GetSystemsByMinRam(IN minRam INT) BEGIN SELECT * FROM Games WHERE MinRam <= minRam; END;");

    // FUNCTIONS
    migrationBuilder.Sql(@"CREATE FUNCTION fn_CalculateSimpleScore(cores INT, vram INT) RETURNS INT DETERMINISTIC READS SQL DATA BEGIN RETURN (cores * 5) + (vram * 10); END;");
    migrationBuilder.Sql(@"CREATE FUNCTION fn_GetGpuDescription(vram INT) RETURNS VARCHAR(50) DETERMINISTIC NO SQL BEGIN IF vram >= 12 THEN RETURN 'High End'; ELSEIF vram >= 8 THEN RETURN 'Mid Range'; ELSE RETURN 'Entry Level'; END IF; END;");
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
