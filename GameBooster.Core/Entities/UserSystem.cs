namespace GameBooster.Core.Entities
{
    public class UserSystem : BaseEntity
    {
        public string SystemName { get; set; } = "My Gaming Rig";
        public int RamAmount { get; set; } // GB cinsinden

        //Sistemin bir kullanıcısı, GPU ve CPU'su olacağını anlatan ilişkiler
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; } = null!;

        public int GPUId { get; set; }
        public GPU GPU { get; set; } = null!;

        public int CPUId { get; set; }
        public CPU CPU { get; set; } = null!;
    }
}

