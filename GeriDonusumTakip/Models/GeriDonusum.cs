namespace GeriDonusumTakip.Models
{
    public class GeriDonusum
    {
        public int Id { get; set; }
        public double Miktar { get; set; }
        public string Tur { get; set; } = string.Empty;
        public DateTime Tarih { get; set; }
        public string? Notlar { get; set; }
        public string? KullaniciId { get; set; }

        // Navigation property
        public UygulamaKullanici? Kullanici { get; set; }
    }
} 