using GeriDonusumTakip.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GeriDonusumTakip.Controllers
{
    public class HesapController : Controller
    {
        private readonly UserManager<UygulamaKullanici> _userManager;
        private readonly SignInManager<UygulamaKullanici> _signInManager;
        private readonly ILogger<HesapController> _logger;

        public HesapController(
            UserManager<UygulamaKullanici> userManager,
            SignInManager<UygulamaKullanici> signInManager,
            ILogger<HesapController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // Kayıt Ol (Register)
        [HttpGet]
        public IActionResult Kayit()
        {
            ViewData["Title"] = "Kayıt Ol";
            return View(new KayitModel());  // Burada KayitModel gönderiliyor
        }

        [HttpPost]
        public async Task<IActionResult> Kayit(KayitModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try 
            {
                var user = new UygulamaKullanici 
                { 
                    UserName = model.Eposta, 
                    Email = model.Eposta,
                    Ad = model.Ad,
                    Soyad = model.Soyad
                };
                
                var result = await _userManager.CreateAsync(user, model.Sifre);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Kullanıcı başarıyla oluşturuldu: {user.Email}");
                    TempData["SuccessMessage"] = $"Hoş geldiniz! {model.Ad} {model.Soyad}, kaydınız başarıyla oluşturuldu. ✨";
                    return RedirectToAction("Giris", "Hesap");
                }

                foreach (var error in result.Errors)
                {
                    _logger.LogError($"Kullanıcı oluşturma hatası: {error.Code} - {error.Description}");
                    ModelState.AddModelError("", error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Kayıt işlemi sırasında hata: {ex}");
                ModelState.AddModelError("", "Kayıt işlemi sırasında bir hata oluştu.");
            }

            return View(model);
        }

        // Giriş Yap (Login)
        [HttpGet]
        public IActionResult Giris()
        {
            var model = new GirisModel(); // GirisModel türünde bir model oluşturuluyor.
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Giris(GirisModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Eposta, model.Sifre, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Panel"); // Giriş başarılıysa Ana Sayfaya yönlendir
                }
                ModelState.AddModelError("", "Geçersiz giriş denemesi");
            }
            return View(model);
        }

        // Çıkış Yap (Logout)
        [HttpPost]
        public async Task<IActionResult> Cikis()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Giris"); // Çıkış yapınca Giriş sayfasına yönlendir
        }
    }
}