using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeriDonusumTakip.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Istatistikler()
        {
            return View();
        }

        public IActionResult Profil()
        {
            return View();
        }
    }
}
