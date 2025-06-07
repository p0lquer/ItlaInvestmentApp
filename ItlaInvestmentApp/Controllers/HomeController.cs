using InvestmentApp.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ItlaInvestmentApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserSession _userSession;

        public HomeController(IUserSession userSession)
        {
            _userSession = userSession;
        }
        public IActionResult Index()
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!_userSession.IsAdmin())
            {
                return RedirectToRoute(new { controller = "Login", action = "AccessDenied" });
            }

            return View();
        }      
    }
}
