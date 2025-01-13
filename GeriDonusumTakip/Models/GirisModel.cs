using System.ComponentModel.DataAnnotations;

namespace GeriDonusumTakip.Models
{
    public class GirisModel
    {
        [Required]
        [EmailAddress]
        public string Eposta { get; set; } = String.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Sifre { get; set; } = String.Empty;
    }
}
