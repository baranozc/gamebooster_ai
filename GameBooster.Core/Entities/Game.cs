using System.ComponentModel.DataAnnotations;

namespace GameBooster.Core.Entities
{
    public class Game : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int MinRam { get; set; }
        public int MinStorage { get; set; }
        public float DemandScore { get; set; }   // Zorluk Puanı (YENİ)
    }
}