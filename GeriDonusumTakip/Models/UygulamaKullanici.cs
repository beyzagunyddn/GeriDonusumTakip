using Microsoft.AspNetCore.Identity;

namespace GeriDonusumTakip.Models
{
    public class UygulamaKullanici : IdentityUser
    {
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public List<string> Rozetler { get; set; } = new List<string>();
        public int ToplamPuan { get; set; }
        
        // Navigation property
        public ICollection<GeriDonusum> GeriDonusumler { get; set; } = new List<GeriDonusum>();
    }
} 