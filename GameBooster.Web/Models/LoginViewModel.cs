
using System.ComponentModel.DataAnnotations;

namespace GameBooster.Web.Models 
{
    public class LoginViewModel 
    {
        [Required(ErrorMessage = "E-posta Alanı Zorunludur.")]
        [EmailAddress(ErrorMessage = "Lütfen Geçerli Bir E-posta Adresi Giriniz.")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Şifre Zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;
        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}