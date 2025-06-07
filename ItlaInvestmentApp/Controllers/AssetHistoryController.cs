using InvestmentApp.Core.Application.Dtos.AssetHistory;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Application.ViewModels.AssetHistory;
using Microsoft.AspNetCore.Mvc;

namespace ItlaInvestmentApp.Controllers
{
    public class AssetHistoryController : Controller
    {
        private readonly IAssetHistoryService _assetHistoryService;
        private readonly IUserSession _userSession;

        public AssetHistoryController(IAssetHistoryService assetHistoryService, IUserSession userSession)
        {
            _assetHistoryService = assetHistoryService;
            _userSession = userSession;
        }

        public IActionResult Create(int assetId)
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!_userSession.IsAdmin())
            {
                return RedirectToRoute(new { controller = "Login", action = "AccessDenied" });
            }

            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }

            return View("Save", new SaveAssetHistoryViewModel() { AssetId = assetId, Id = 0, Value = 0,HistoryValueDate = DateTime.UtcNow });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveAssetHistoryViewModel vm)
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!_userSession.IsAdmin())
            {
                return RedirectToRoute(new { controller = "Login", action = "AccessDenied" });
            }

            if (!ModelState.IsValid)
            {
                return View("Save", vm);
            }

            AssetHistoryDto dto = new()
            {
                Id = 0,
                AssetId = vm.AssetId,
                Value = vm.Value,
                HistoryValueDate = vm.HistoryValueDate
            };

            await _assetHistoryService.AddAsync(dto);
            return RedirectToRoute(new { controller = "Asset", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!_userSession.IsAdmin())
            {
                return RedirectToRoute(new { controller = "Login", action = "AccessDenied" });
            }

            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }

            ViewBag.EditMode = true;
            var dto = await _assetHistoryService.GetById(id);

            if (dto == null)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }

            SaveAssetHistoryViewModel vm = new()
            {
                Id = dto.Id,
                AssetId = dto.AssetId,
                Value = dto.Value,
                HistoryValueDate = dto.HistoryValueDate,
            };

            return View("Save", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveAssetHistoryViewModel vm)
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!_userSession.IsAdmin())
            {
                return RedirectToRoute(new { controller = "Login", action = "AccessDenied" });
            }

            if (!ModelState.IsValid)
            {
                ViewBag.EditMode = true;
                ViewBag.AssetTypes = await _assetHistoryService.GetAll();
                return View("Save", vm);
            }

            AssetHistoryDto dto = new()
            {
                Id = vm.Id,
                AssetId = vm.AssetId,
                Value = vm.Value,
                HistoryValueDate = vm.HistoryValueDate,                
            };
            await _assetHistoryService.UpdateAsync(dto);
            return RedirectToRoute(new { controller = "Asset", action = "Index" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!_userSession.IsAdmin())
            {
                return RedirectToRoute(new { controller = "Login", action = "AccessDenied" });
            }

            if (!ModelState.IsValid)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }

            var dto = await _assetHistoryService.GetById(id);
            if (dto == null)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }
            DeleteAssetHistoryViewModel vm = new() { Id = dto.Id, HistoryValueDate = dto.HistoryValueDate.ToShortDateString() };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteAssetHistoryViewModel vm)
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!_userSession.IsAdmin())
            {
                return RedirectToRoute(new { controller = "Login", action = "AccessDenied" });
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            await _assetHistoryService.DeleteAsync(vm.Id);
            return RedirectToRoute(new { controller = "Asset", action = "Index" });
        }
    }
}
