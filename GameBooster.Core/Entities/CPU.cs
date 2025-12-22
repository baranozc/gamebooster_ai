using System.ComponentModel.DataAnnotations;

namespace GameBooster.Core.Entities
{
    public class CPU : BaseEntity
    {
        public string? Name { get; set; }
        public int CoreCount { get; set; }      // Çekirdek
        public int ThreadCount { get; set; }    // İş Parçacığı (YENİ)
        public double MaxFrequency { get; set; } // Hız
        public int Cache { get; set; }          // Önbellek (YENİ)
    }
}