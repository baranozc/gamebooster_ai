using System;
using Microsoft.AspNetCore.Identity;
namespace GameBooster.Core.Entities
{
    public class AppUser : IdentityUser<int>
    {
        // Identity User içerisinde zaten Username,email,passwordHash gibi kısımlar bulunuyor.
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Kullanıcının sistemleri
        public ICollection<UserSystem> Systems { get; set; } = new List<UserSystem>();
    }
}

