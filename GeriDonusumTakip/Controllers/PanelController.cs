using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using GeriDonusumTakip.Data;
using GeriDonusumTakip.Models;

namespace GeriDonusumTakip.Controllers
{
    [Authorize]
    public class PanelController : Controller
    {
        private readonly UygulamaDbContext _context;

        public PanelController(UygulamaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var kayitlar = await _context.GeriDonusumler
                .Where(g => g.KullaniciId == userId)
                .ToListAsync();

            var (agac, su, enerji, emisyon) = HesaplaEtki(kayitlar);

            var viewModel = new DashboardViewModel
            {
                ToplamGeriDonusum = kayitlar.Sum(x => x.Miktar),
                KurtarilanAgac = agac,
                TasarrufEdilenSu = su,
                TasarrufEdilenEnerji = enerji,
                OnlenenEmisyon = emisyon
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GeriDonusumKayit()
        {
            var model = new GeriDonusum
            {
                Tarih = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GeriDonusumKayit(GeriDonusum model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var kullanici = await _context.Users.FindAsync(userId);

            model.KullaniciId = userId;
            model.Tarih = DateTime.Now;
            _context.GeriDonusumler.Add(model);

            var tumKayitlar = await _context.GeriDonusumler
                .Where(g => g.KullaniciId == userId)
                .ToListAsync();

            var kayitSayisi = tumKayitlar.Count + 1;
            var (agac, su, enerji, emisyon) = HesaplaEtki(tumKayitlar);

            // Mevcut rozetler...

            // Enerji Tasarrufu Rozeti ⚡ (1000 kWh)
            if (enerji >= 1000 && !kullanici.Rozetler.Contains("EnerjiTasarrufu"))
            {
                kullanici.Rozetler.Add("EnerjiTasarrufu");
                kullanici.ToplamPuan += 150;
                TempData["YeniRozet"] = "⚡ Enerji Tasarrufu Rozeti kazandınız!";
            }

            // CO2 Önleme Rozeti 🌍 (500 kg CO2)
            if (emisyon >= 500 && !kullanici.Rozetler.Contains("EmisyonOnleme"))
            {
                kullanici.Rozetler.Add("EmisyonOnleme");
                kullanici.ToplamPuan += 150;
                TempData["YeniRozet"] = "🌍 CO2 Önleme Rozeti kazandınız!";
            }

            // Cam Geri Dönüşüm Rozeti 🔍 (100 kg cam)
            var toplamCam = tumKayitlar.Where(g => g.Tur.Equals("cam", StringComparison.OrdinalIgnoreCase)).Sum(g => g.Miktar);
            if (model.Tur.Equals("cam", StringComparison.OrdinalIgnoreCase)) toplamCam += model.Miktar;

            if (toplamCam >= 100 && !kullanici.Rozetler.Contains("CamDostu"))
            {
                kullanici.Rozetler.Add("CamDostu");
                kullanici.ToplamPuan += 100;
                TempData["YeniRozet"] = "🔍 Cam Dostu Rozeti kazandınız!";
            }

            // Metal Geri Dönüşüm Rozeti ⚙️ (50 kg metal)
            var toplamMetal = tumKayitlar.Where(g => g.Tur.Equals("metal", StringComparison.OrdinalIgnoreCase)).Sum(g => g.Miktar);
            if (model.Tur.Equals("metal", StringComparison.OrdinalIgnoreCase)) toplamMetal += model.Miktar;

            if (toplamMetal >= 50 && !kullanici.Rozetler.Contains("MetalDostu"))
            {
                kullanici.Rozetler.Add("MetalDostu");
                kullanici.ToplamPuan += 100;
                TempData["YeniRozet"] = "⚙️ Metal Dostu Rozeti kazandınız!";
            }

                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Geri dönüşüm kaydınız başarıyla eklendi! 🌱";
            return RedirectToAction(nameof(GeriDonusumKayit));
        }

        public async Task<IActionResult> Istatistikler()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var kayitlar = await _context.GeriDonusumler
                .Where(g => g.KullaniciId == userId)
                .ToListAsync();

            // Debug için eklenen log
            var aktifHedefler = await _context.KisiselHedefler
                .Where(h => h.KullaniciId == userId && !h.Tamamlandi)
                .ToListAsync();

            System.Diagnostics.Debug.WriteLine($"Aktif hedef sayısı: {aktifHedefler.Count}");
            foreach (var hedef in aktifHedefler)
            {
                System.Diagnostics.Debug.WriteLine($"Hedef: {hedef.HedefTuru}, {hedef.HedefMiktar} kg");
            }

            var hedefDurumlari = aktifHedefler.Select(hedef =>
            {
                var guncelMiktar = kayitlar
                    .Where(k => k.Tur.Equals(hedef.HedefTuru, StringComparison.OrdinalIgnoreCase) &&
                               k.Tarih >= hedef.BaslangicTarihi)
                    .Sum(k => k.Miktar);

                return new HedefDurumu
                {
                    Hedef = hedef,
                    GuncelMiktar = guncelMiktar
                };
            }).ToList();

            var (agac, su, enerji, emisyon) = HesaplaEtki(kayitlar);

            // Aylık etki gelişimini hesapla
            var aylikEtkiler = kayitlar
                .GroupBy(g => new DateTime(g.Tarih.Year, g.Tarih.Month, 1))
                .OrderByDescending(g => g.Key)
                .Take(6)  // Son 6 ay
                .Select(g =>
                {
                    var (agacAy, suAy, enerjiAy, _) = HesaplaEtki(g);
                    return new EtkiGelisimi
                    {
                        Tarih = g.Key,
                        KurtarilanAgac = agacAy,
                        TasarrufEdilenSu = suAy,
                        TasarrufEdilenEnerji = enerjiAy
                    };
                })
                .ToList();

            var model = new IstatistikViewModel
            {
                ToplamKayit = kayitlar.Count,
                ToplamMiktar = kayitlar.Sum(g => g.Miktar),
                KurtarilanAgac = agac,
                TasarrufEdilenSu = su,
                TasarrufEdilenEnerji = enerji,
                OnlenenEmisyon = emisyon,

                TurBazliDagilim = kayitlar
                    .GroupBy(g => g.Tur)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Miktar)),

                KagitEtki = HesaplaTurEtki(kayitlar, "kagit"),
                PlastikEtki = HesaplaTurEtki(kayitlar, "plastik"),
                CamEtki = HesaplaTurEtki(kayitlar, "cam"),
                MetalEtki = HesaplaTurEtki(kayitlar, "metal"),

                EtkiGelisimi = aylikEtkiler,  // Aylık etki gelişimi için
                AktifHedefler = hedefDurumlari
            };

            return View(model);
        }

        private CevreselEtki HesaplaTurEtki(IEnumerable<GeriDonusum> kayitlar, string tur)
        {
            var toplamMiktar = kayitlar
                .Where(g => g.Tur.Equals(tur, StringComparison.OrdinalIgnoreCase) || 
                            (tur == "kagit" && g.Tur.Equals("kağıt", StringComparison.OrdinalIgnoreCase)))
                .Sum(g => g.Miktar);

            return tur.ToLower() switch
            {
                "kagit" => new CevreselEtki
                {
                    Agac = toplamMiktar * 0.017,
                    Su = toplamMiktar * 26,
                    Enerji = toplamMiktar * 4.1,
                    Emisyon = toplamMiktar * 2.5
                },
                "plastik" => new CevreselEtki
                {
                    Su = toplamMiktar * 15,
                    Enerji = toplamMiktar * 5.8,
                    Emisyon = toplamMiktar * 3.1
                },
                "cam" => new CevreselEtki
                {
                    Su = toplamMiktar * 8,
                    Enerji = toplamMiktar * 2.5,
                    Emisyon = toplamMiktar * 0.9
                },
                "metal" => new CevreselEtki
                {
                    Su = toplamMiktar * 12,
                    Enerji = toplamMiktar * 14,
                    Emisyon = toplamMiktar * 4.5
                },
                _ => new CevreselEtki()
            };
        }

        public async Task<IActionResult> Profil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var kullanici = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            return View(kullanici);
        }

        public async Task<IActionResult> GeriDonusumListesi()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var kayitlar = await _context.GeriDonusumler
                .Where(g => g.KullaniciId == userId)
                .OrderByDescending(g => g.Tarih)
                .ToListAsync();

            return View(kayitlar);
        }

        [HttpPost]
        public async Task<IActionResult> GeriDonusumSil(int id)
        {
            var kayit = await _context.GeriDonusumler.FindAsync(id);
            if (kayit != null && kayit.KullaniciId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                _context.GeriDonusumler.Remove(kayit);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Kayıt başarıyla silindi.";
            }
            return RedirectToAction(nameof(GeriDonusumListesi));
        }

        private (double agac, double su, double enerji, double emisyon) HesaplaEtki(IEnumerable<GeriDonusum> kayitlar)
        {
            // Türlere göre toplam miktarları hesapla 
            var toplamKagit = kayitlar
                .Where(g => g.Tur.Equals("kagit", StringComparison.OrdinalIgnoreCase) || 
                            g.Tur.Equals("kağıt", StringComparison.OrdinalIgnoreCase))
                .Sum(g => g.Miktar);

            var toplamPlastik = kayitlar
                .Where(g => g.Tur.Equals("plastik", StringComparison.OrdinalIgnoreCase) || 
                            g.Tur.Equals("plastic", StringComparison.OrdinalIgnoreCase))
                .Sum(g => g.Miktar);

            var toplamCam = kayitlar
                .Where(g => g.Tur.Equals("cam", StringComparison.OrdinalIgnoreCase))
                .Sum(g => g.Miktar);

            var toplamMetal = kayitlar
                .Where(g => g.Tur.Equals("metal", StringComparison.OrdinalIgnoreCase))
                .Sum(g => g.Miktar);

            // Kağıt etkisi
            var kagitAgac = toplamKagit * 0.017;     // Her kg kağıt 0.017 ağaç
            var kagitSu = toplamKagit * 26;          // Her kg kağıt 26 litre su
            var kagitEnerji = toplamKagit * 4.1;     // Her kg kağıt 4.1 kWh enerji
            var kagitEmisyon = toplamKagit * 2.5;    // Her kg kağıt 2.5 kg CO2

            // Plastik etkisi
            var plastikSu = toplamPlastik * 15;      // Her kg plastik 15 litre su
            var plastikEnerji = toplamPlastik * 5.8; // Her kg plastik 5.8 kWh enerji
            var plastikEmisyon = toplamPlastik * 3.1;// Her kg plastik 3.1 kg CO2

            // Cam etkisi
            var camSu = toplamCam * 8;               // Her kg cam 8 litre su
            var camEnerji = toplamCam * 2.5;         // Her kg cam 2.5 kWh enerji
            var camEmisyon = toplamCam * 0.9;        // Her kg cam 0.9 kg CO2

            // Metal etkisi
            var metalSu = toplamMetal * 12;          // Her kg metal 12 litre su
            var metalEnerji = toplamMetal * 14;      // Her kg metal 14 kWh enerji
            var metalEmisyon = toplamMetal * 4.5;    // Her kg metal 4.5 kg CO2

            return (
                agac: kagitAgac,
                su: kagitSu + plastikSu + camSu + metalSu,
                enerji: kagitEnerji + plastikEnerji + camEnerji + metalEnerji,
                emisyon: kagitEmisyon + plastikEmisyon + camEmisyon + metalEmisyon
            );
        }

        // Geçici test metodu
        public async Task<IActionResult> RozetKontrol()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var kullanici = await _context.Users.FindAsync(userId);
            var tumKayitlar = await _context.GeriDonusumler
                .Where(g => g.KullaniciId == userId)
                .ToListAsync();

            var (agac, su, enerji, emisyon) = HesaplaEtki(tumKayitlar);
            var toplamCam = tumKayitlar.Where(g => g.Tur.Equals("cam", StringComparison.OrdinalIgnoreCase)).Sum(g => g.Miktar);
            var toplamMetal = tumKayitlar.Where(g => g.Tur.Equals("metal", StringComparison.OrdinalIgnoreCase)).Sum(g => g.Miktar);

            // Mevcut rozet kontrolleri...

            // Yeni rozet kontrolleri
            if (enerji >= 1000 && !kullanici.Rozetler.Contains("EnerjiTasarrufu"))
            {
                kullanici.Rozetler.Add("EnerjiTasarrufu");
                kullanici.ToplamPuan += 150;
            }

            if (emisyon >= 500 && !kullanici.Rozetler.Contains("EmisyonOnleme"))
            {
                kullanici.Rozetler.Add("EmisyonOnleme");
                kullanici.ToplamPuan += 150;
            }

            if (toplamCam >= 100 && !kullanici.Rozetler.Contains("CamDostu"))
            {
                kullanici.Rozetler.Add("CamDostu");
                kullanici.ToplamPuan += 100;
            }

            if (toplamMetal >= 50 && !kullanici.Rozetler.Contains("MetalDostu"))
            {
                kullanici.Rozetler.Add("MetalDostu");
                kullanici.ToplamPuan += 100;
            }

            await _context.SaveChangesAsync();

            return Json(new { 
                KayitSayisi = tumKayitlar.Count,
                MevcutRozetler = kullanici.Rozetler,
                Enerji = enerji,
                Emisyon = emisyon,
                ToplamCam = toplamCam,
                ToplamMetal = toplamMetal
            });
        }

        // Kayıt sayısı kontrolü için test metodu
        public async Task<IActionResult> KayitKontrol()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var kayitlar = await _context.GeriDonusumler
                .Where(g => g.KullaniciId == userId)
                .Select(g => new { 
                    g.Id, 
                    g.Tarih, 
                    g.Tur, 
                    g.Miktar 
                })
                .OrderBy(g => g.Tarih)
                .ToListAsync();

            return Json(new { 
                KayitSayisi = kayitlar.Count,
                Kayitlar = kayitlar
            });
        }

        [HttpPost]
        public async Task<IActionResult> HedefEkle(KisiselHedef hedef)
        {
            try
            {
                hedef.KullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                hedef.BaslangicTarihi = DateTime.Now;
                hedef.Tamamlandi = false;

                _context.KisiselHedefler.Add(hedef);
                await _context.SaveChangesAsync();

                TempData["Mesaj"] = $"Hedef eklendi: {hedef.HedefTuru}, {hedef.HedefMiktar} kg";
            }
            catch (Exception ex)
            {
                TempData["Hata"] = $"Hata oluştu: {ex.Message}";
            }

            return RedirectToAction(nameof(Istatistikler));
        }

        [HttpPost]
        public async Task<IActionResult> HedefSil(int id)
        {
            var hedef = await _context.KisiselHedefler.FindAsync(id);
            if (hedef?.KullaniciId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                _context.KisiselHedefler.Remove(hedef);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Hedef silindi.";
            }
            return RedirectToAction(nameof(Istatistikler));
        }
    }
}