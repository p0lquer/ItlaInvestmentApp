using InvestmentApp.Core.Application.Dtos.AssetType;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Application.ViewModels.AssetType;
using Microsoft.AspNetCore.Mvc;

namespace ItlaInvestmentApp.Controllers
{
    public class AssetTypeController : Controller
    {
        private readonly IAssetTypeService _assetTypeService;
        private readonly IUserSession _userSession;

        public AssetTypeController(IAssetTypeService assetTypeService, IUserSession userSession)
        {
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

            var dtos = await _assetTypeService.GetAllWithInclude();

            var listEntityVms = dtos.Select(s =>
              new AssetTypeViewModel()
              {
                  Id = s.Id,
                  Name = s.Name,
                  Description = s.Description,
                  AssetQuantity = s.AssetQuantity
              }).ToList();

            return View(listEntityVms);
        }

        public IActionResult Create()
        {
            if (!_userSession.HasUser())
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (!_userSession.IsAdmin())
            {
                return RedirectToRoute(new { controller = "Login", action = "AccessDenied" });
            }

            return View("Save", new SaveAssetTypeViewModel() { Name = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveAssetTypeViewModel vm)
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

            AssetTypeDto dto = new() { Id = 0, Name = vm.Name, Description = vm.Description };
            await _assetTypeService.AddAsync(dto);
            return RedirectToRoute(new { controller = "AssetType", action = "Index" });
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
                return RedirectToRoute(new { controller = "AssetType", action = "Index" });
            }

            ViewBag.EditMode = true;
            var dto = await _assetTypeService.GetById(id);
            if (dto == null)
            {
                return RedirectToRoute(new { controller = "AssetType", action = "Index" });
            }
            SaveAssetTypeViewModel vm = new() { Id = dto.Id, Name = dto.Name, Description = dto.Description };
            return View("Save", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveAssetTypeViewModel vm)
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
                return View("Save", vm);
            }

            AssetTypeDto dto = new() { Id = vm.Id, Name = vm.Name, Description = vm.Description };
            await _assetTypeService.UpdateAsync(dto);
            return RedirectToRoute(new { controller = "AssetType", action = "Index" });
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
                return RedirectToRoute(new { controller = "AssetType", action = "Index" });
            }

            var dto = await _assetTypeService.GetById(id);
            if (dto == null)
            {
                return RedirectToRoute(new { controller = "AssetType", action = "Index" });
            }
            DeleteAssetTypeViewModel vm = new() { Id = dto.Id, Name = dto.Name };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteAssetTypeViewModel vm)
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

            await _assetTypeService.DeleteAsync(vm.Id);
            return RedirectToRoute(new { controller = "AssetType", action = "Index" });
        }

    }
}
