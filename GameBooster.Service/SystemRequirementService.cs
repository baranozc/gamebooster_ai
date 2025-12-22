
using GameBooster.Core.Interfaces;
using Microsoft.Identity.Client;

namespace GameBooster.Service 
{
    public class SystemRequirementService : ISystemRequirementService
    {
        public string CheckRequirement(string gameName, int ramAmount)
        {
            string game = gameName.ToLower();

            int minRam = 8;

            if(game.Contains("cyberpunk") || game.Contains("rdr2") || game.Contains("red dead")) 
            {
                minRam = 16;
            }
            else if(game.Contains("warzone") || game.Contains("call of duty")) 
            {
                minRam = 12;
            }

            if(ramAmount >= minRam) 
            {
                return $"✅ BAŞARILI: {ramAmount}GB RAM, {gameName} için yeterli. (Min: {minRam}GB)";
            }
            else
            {
                return $"❌ YETERSİZ: {gameName} oynamak için en az {minRam}GB RAM gerekiyor!";
            }
        }
    }

}