using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Application.ViewModels.Asset;
using InvestmentApp.Core.Application.ViewModels.AssetHistory;
using InvestmentApp.Core.Application.ViewModels.AssetType;
using Microsoft.AspNetCore.Mvc;

namespace ItlaInvestmentApp.Controllers
{
    public class AssetController : Controller
    {
        private readonly IAssetService _assetService;
        private readonly IAssetTypeService _assetTypeService;
        private readonly IUserSession _userSession;

        public AssetController(IAssetService assetService, IAssetTypeService assetTypeService, IUserSession userSession)
        {
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

            if (!_userSession.IsAdmin())
            {
                return RedirectToRoute(new { controller = "Login", action = "AccessDenied" });
            }

            var dtos = await _assetService.GetAllWithInclude();

            var listEntityVms = dtos.Select(s =>
              new AssetViewModel()
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
                  AssetHistories = s.AssetHistories == null
                    ? []
                    : s.AssetHistories                   
                    .Select(s => new AssetHistoryViewModel()
                    {
                        AssetId = s.AssetId,
                        Id = s.Id,
                        HistoryValueDate = s.HistoryValueDate,
                        Value = s.Value
                    }).ToList()
              }).ToList();

            return View(listEntityVms);
        }

        public async Task<IActionResult> Create()
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!_userSession.IsAdmin())
            {
                return RedirectToRoute(new { controller = "Login", action = "AccessDenied" });
            }

            ViewBag.AssetTypes = await _assetTypeService.GetAll();
            return View("Save", new SaveAssetViewModel() { Name = "", Symbol = "", AssetTypeId = null });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveAssetViewModel vm)
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
                ViewBag.AssetTypes = await _assetTypeService.GetAll();
                return View("Save", vm);
            }

            AssetDto dto = new()
            {
                Id = 0,
                Name = vm.Name,
                Description = vm.Description,
                AssetTypeId = vm.AssetTypeId ?? 0,
                Symbol = vm.Symbol
            };

            await _assetService.AddAsync(dto);
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
            ViewBag.AssetTypes = await _assetTypeService.GetAll();
            var dto = await _assetService.GetById(id);

            if (dto == null)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }

            SaveAssetViewModel vm = new()
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Symbol = dto.Symbol,
                AssetTypeId = dto.AssetTypeId
            };
            return View("Save", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveAssetViewModel vm)
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
                ViewBag.AssetTypes = await _assetTypeService.GetAll();
                return View("Save", vm);
            }

            AssetDto dto = new()
            {
                Id = vm.Id,
                Name = vm.Name,
                Description = vm.Description,
                Symbol = vm.Symbol,
                AssetTypeId = vm.AssetTypeId ?? 0
            };
            await _assetService.UpdateAsync(dto);
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
            var dto = await _assetService.GetById(id);
            if (dto == null)
            {
                return RedirectToRoute(new { controller = "Asset", action = "Index" });
            }
            DeleteAssetViewModel vm = new() { Id = dto.Id, Name = dto.Name };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteAssetViewModel vm)
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

            await _assetService.DeleteAsync(vm.Id);
            return RedirectToRoute(new { controller = "Asset", action = "Index" });
        }

    }
}
