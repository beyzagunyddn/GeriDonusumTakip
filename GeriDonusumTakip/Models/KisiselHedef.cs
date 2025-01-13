using System;

namespace GeriDonusumTakip.Models
{
    public class KisiselHedef
    {
        public int Id { get; set; }
        public string KullaniciId { get; set; }
        public string HedefTuru { get; set; }  // "Kagit", "Plastik", "Cam", "Metal"
        public double HedefMiktar { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public bool Tamamlandi { get; set; }
    }
} 