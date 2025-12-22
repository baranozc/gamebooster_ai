
using System.ComponentModel.DataAnnotations;

namespace GameBooster.Web.Models 
{
    public class RegisterViewModel 
    {
        [Required(ErrorMessage = "Ad Alanı Zorunludur.")]
        [Display(Name = "Adınız")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Soyad Alanı Zorunludur.")]
        [Display(Name = "Soyadınız")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "E-posta Alanı Zorunludur.")]
        [EmailAddress(ErrorMessage = "Lütfen Geçerli Bir E-posta Adresi Giriniz.")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Şifre Zorunludur.")]
        [DataType(DataType.Password)]
        [MinLength(3, ErrorMessage = "Şifre en az 3 karakter olmalıdır.")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Şifre Tekrarı Zorunludur.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler Uyuşmuyor.")]
        [Display(Name = "Şifre Tekrar")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}