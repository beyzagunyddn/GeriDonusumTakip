using System.ComponentModel.DataAnnotations;

namespace GeriDonusumTakip.Models
{
    public class KayitModel
    {
        [Required(ErrorMessage = "Ad alanı boş bırakılamaz")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı boş bırakılamaz")]
        public string Soyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı boş bırakılamaz")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin")]
        public string Eposta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [StringLength(100, ErrorMessage = "Şifre en az 6 karakter olmalıdır", MinimumLength = 6)]
        public string Sifre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre onayı gereklidir")]
        [Compare("Sifre", ErrorMessage = "Şifreler eşleşmiyor")]
        public string SifreOnay { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        [Display(Name = "Telefon Numarası")]
        public string? Telefon { get; set; }
    }
}
    

