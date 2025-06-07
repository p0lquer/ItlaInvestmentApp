using InvestmentApp.Core.Application.Dtos.InvestmentAssets;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Application.ViewModels.InvestmentAssets;
using Microsoft.AspNetCore.Mvc;

namespace ItlaInvestmentApp.Controllers
{
    public class InvestmentAssetController : Controller
    {
        private readonly IInvestmentAssetsService _investmentAssetsService;
        private readonly IAssetService _assetService;
        private readonly IUserSession _userSession;

        public InvestmentAssetController(IInvestmentAssetsService investmentAssetsService, IAssetService assetService, IUserSession userSession)
        {
            _investmentAssetsService = investmentAssetsService;
            _assetService = assetService;
            _userSession = userSession;
        }
        public async Task<IActionResult> Create(int portfolioId)
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", portfolioId });
            }

            ViewBag.Assets = await _assetService.GetAll();
            return View(new SaveInvestmentAssetViewModel() { AssetId = 0, Id = 0, InvestmentPortfolioId = portfolioId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveInvestmentAssetViewModel vm)
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Assets = await _assetService.GetAll();
                return View(vm);
            }

            InvestmentAssetsDto dto = new()
            {
                Id = 0,
                AssetId = vm.AssetId,
                InvestmentPortfolioId = vm.InvestmentPortfolioId
            };

            await _investmentAssetsService.AddAsync(dto);
            return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", portfolioId = vm.InvestmentPortfolioId });
        }

        public async Task<IActionResult> Delete(int assetId, int portfolioId)
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", portfolioId });
            }

            var dto = await _investmentAssetsService.GetByAssetAndPortfolioAsync(assetId, portfolioId);
            if (dto == null)
            {
                return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", portfolioId });
            }

            DeleteInvestmentAssetViewModel vm = new()
            {
                Id = dto.Id,
                AssetName = dto.Asset?.Name,
                PortfolioId = dto.InvestmentPortfolioId,
                AssetId = dto.AssetId
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteInvestmentAssetViewModel vm)
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            await _investmentAssetsService.DeleteAsync(vm.Id);
            return RedirectToRoute(new { controller = "InvestmentPortfolio", action = "AssetsDetails", portfolioId = vm.PortfolioId });
        }
    }
}
