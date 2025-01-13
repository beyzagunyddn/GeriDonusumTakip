using Microsoft.AspNetCore.Identity;

namespace GeriDonusumTakip.Models
{
    public class AppUser : IdentityUser
    {
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        
        // Navigation property
        public ICollection<GeriDonusum> GeriDonusumler { get; set; } = new List<GeriDonusum>();
    }
} 