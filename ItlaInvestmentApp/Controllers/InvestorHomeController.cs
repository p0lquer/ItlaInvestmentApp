using InvestmentApp.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ItlaInvestmentApp.Controllers
{
    public class InvestorHomeController : Controller
    {
        private readonly IUserSession _userSession;
        public InvestorHomeController(IUserSession userSession)
        {
            _userSession = userSession;
        }

        public IActionResult Index()
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (_userSession.IsAdmin())
            {
                return RedirectToRoute(new { controller = "Login", action = "AccessDenied" });
            }

            return View();
        }      
    }
}
