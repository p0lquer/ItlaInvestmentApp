using InvestmentApp.Core.Application.Dtos.InvestmentPortfolio;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Application.ViewModels.Asset;
using InvestmentApp.Core.Application.ViewModels.AssetType;
using InvestmentApp.Core.Application.ViewModels.InvestmentPortfolio;
using InvestmentApp.Core.Application.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace ItlaInvestmentApp.Controllers
{
    public class InvestmentPortfolioController : Controller
    {
        private readonly IInvestmentPortfolioService _investmentPortfolioService;    
        private readonly IAssetService _assetService;
        private readonly IAssetTypeService _assetTypeService;
        private readonly IUserSession _userSession;
        public InvestmentPortfolioController(IInvestmentPortfolioService investmentPortfolioService, IAssetService assetService, IAssetTypeService assetTypeService, IUserSession userSession)
        {
            _investmentPortfolioService = investmentPortfolioService;            
            _assetService = assetService;
            _assetTypeService = assetTypeService;
            _userSession = userSession;
        }
        public async Task<IActionResult> Index()
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }
            UserViewModel? userSession = _userSession.GetUserSession();
            if (userSession == null) {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            var dtos = await _investmentPortfolioService.GetAllWithIncludeByUser(userSession.Id);

            var listEntityVms = dtos.Select(s =>
              new InvestmentPortfolioViewModel()
              {
                  Id = s.Id,
                  Name = s.Name,
                  Description = s.Description,
                  UserId = s.UserId,
                  User = s.User == null ? null : new UserViewModel()
                  {
                      Id = s.User.Id,
                      Name = s.User.Name,
                      Email = s.User.Email,
                      UserName = s.User.UserName,
                      LastName = s.User.LastName,
                      Role = s.User.Role,
                      Phone = s.User.Phone,
                      ProfileImage = s.User.ProfileImage
                  }
              }).ToList();

            return View(listEntityVms);
        }

        public IActionResult Create()
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }            

            return View("Save", new SaveInvestmentPortfolioViewModel() { Name = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveInvestmentPortfolioViewModel vm)
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!ModelState.IsValid)
            {             
                return View("Save", vm);
            }

            UserViewModel? userSession = _userSession.GetUserSession();
            if (userSession == null)
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            InvestmentPortfolioDto dto = new()
            {
                Id = 0,
                Name = vm.Name,
                Description = vm.Description,
                UserId = userSession.Id,
            };

            await _investmentPortfolioService.AddAsync(dto);
            return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
        }

        public async Task<IActionResult> AssetsDetails(int portfolioId,string? assetName = null,int? assetTypeId = null,int? assetOrderBy = null )
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!ModelState.IsValid)
            {              
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
            }

            var portfolioDto = await _investmentPortfolioService.GetById(portfolioId);

            if (portfolioDto == null)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
            }

            InvestmentPortfolioViewModel portfolioVm = new()
            {
                Id = portfolioDto.Id,
                Name = portfolioDto.Name,
                Description = portfolioDto.Description,
                UserId = portfolioDto.UserId
            };

            var dtos = await _assetService.GetAllAssetsByPortfolioId(portfolioId,assetName,assetTypeId,assetOrderBy);

            var listEntityVms = dtos.Select(s =>
         new AssetForPortfolioViewModel()
         {
             Id = s.Id,
             Name = s.Name,
             Description = s.Description,
             Symbol = s.Symbol,
             AssetTypeId = s.AssetTypeId,
             AssetType = s.AssetType == null ? null : new AssetTypeViewModel()
             {
                 Id = s.AssetType.Id,
                 Name = s.AssetType.Name,
                 Description = s.AssetType.Description
             },
             CurrentValue = s.CurrentValue,
         }).ToList();

            ViewBag.Portfolio = portfolioVm;
            ViewBag.AssetTypes = await _assetTypeService.GetAll();

            return View("Details", listEntityVms);
        }   
        public async Task<IActionResult> Edit(int id)
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
            }

            ViewBag.EditMode = true;          
            var dto = await _investmentPortfolioService.GetById(id);

            if (dto == null)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
            }       

            SaveInvestmentPortfolioViewModel vm = new()
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,               
            };

            return View("Save", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveInvestmentPortfolioViewModel vm)
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!ModelState.IsValid)
            {
                ViewBag.EditMode = true;               
                return View("Save", vm);
            }

            UserViewModel? userSession = _userSession.GetUserSession();
            if (userSession == null)
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            InvestmentPortfolioDto dto = new()
            {
                Id = vm.Id,
                Name = vm.Name,
                Description = vm.Description,
                UserId = userSession.Id
            };
            await _investmentPortfolioService.UpdateAsync(dto);
            return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
            }
            var dto = await _investmentPortfolioService.GetById(id);
            if (dto == null)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
            }
            DeleteInvestmentPortfolioViewModel vm = new() { Id = dto.Id, Name = dto.Name };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteInvestmentPortfolioViewModel vm)
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            await _investmentPortfolioService.DeleteAsync(vm.Id);
            return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "Index" });
        }

    }
}
