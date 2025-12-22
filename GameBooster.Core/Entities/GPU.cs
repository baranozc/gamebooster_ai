using System.ComponentModel.DataAnnotations;

namespace GameBooster.Core.Entities
{
    public class GPU : BaseEntity
    {
        public string? Name { get; set; }
        public int Vram { get; set; }
        public int CoreCount { get; set; }      // CUDA Çekirdeği (YENİ)
    }
}