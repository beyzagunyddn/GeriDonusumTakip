using System;
using System.Collections.Generic;

namespace GeriDonusumTakip.Models
{
    public class IstatistikViewModel
    {
        public IstatistikViewModel()
        {
            TurBazliDagilim = new Dictionary<string, double>();
            EtkiGelisimi = new List<EtkiGelisimi>();
            AktifHedefler = new List<HedefDurumu>();
            KagitEtki = new CevreselEtki();
            PlastikEtki = new CevreselEtki();
            CamEtki = new CevreselEtki();
            MetalEtki = new CevreselEtki();
        }

        public int ToplamKayit { get; set; }
        public double ToplamMiktar { get; set; }
        public double KurtarilanAgac { get; set; }
        public double TasarrufEdilenSu { get; set; }
        public double TasarrufEdilenEnerji { get; set; }
        public double OnlenenEmisyon { get; set; }
        public Dictionary<string, double> TurBazliDagilim { get; set; }
        public CevreselEtki KagitEtki { get; set; }
        public CevreselEtki PlastikEtki { get; set; }
        public CevreselEtki CamEtki { get; set; }
        public CevreselEtki MetalEtki { get; set; }
        public double YillikHedefYuzdesi => (ToplamMiktar / 1000) * 100;
        public double AgacHedefYuzdesi => (KurtarilanAgac / 100) * 100;
        public List<EtkiGelisimi> EtkiGelisimi { get; set; }
        public List<HedefDurumu> AktifHedefler { get; set; }
    }

    public class HedefDurumu
    {
        public KisiselHedef Hedef { get; set; }
        public double GuncelMiktar { get; set; }
        public double TamamlanmaYuzdesi => (GuncelMiktar / Hedef.HedefMiktar) * 100;
        public int KalanGun => (Hedef.BitisTarihi - DateTime.Now).Days;
    }
} 