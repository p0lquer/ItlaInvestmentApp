using InvestmentApp.Core.Application.Dtos.User;
using InvestmentApp.Core.Application.Helpers;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Application.ViewModels.User;
using InvestmentApp.Core.Domain.Common.Enums;
using ItlaInvestmentApp.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ItlaInvestmentApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserSession _userSession;

        public LoginController(IUserService userService, IUserSession userSession)
        {
            _userService = userService;
            _userSession = userSession;
        }
        public IActionResult Index()
        {
            if (_userSession.HasUser())
            {
                UserViewModel? userSession = _userSession.GetUserSession();
                if (userSession != null)
                {
                    return userSession.Role switch
                    {
                        (int)Role.ADMIN => RedirectToRoute(new { controller = "Home", action = "Index" }),
                        (int)Role.INVESTOR => RedirectToRoute(new { controller = "InvestorHome", action = "Index" }),
                        _ => RedirectToRoute(new { controller = "Login", action = "Index" }),
                    };
                }
            }

            return View(new LoginViewModel() { Password = "", UserName = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel vm)
        {
            if (_userSession.HasUser())
            {
                UserViewModel? userSession = _userSession.GetUserSession();
                if (userSession != null)
                {
                    return userSession.Role switch
                    {
                        (int)Role.ADMIN => RedirectToRoute(new { controller = "Home", action = "Index" }),
                        (int)Role.INVESTOR => RedirectToRoute(new { controller = "InvestorHome", action = "Index" }),
                        _ => RedirectToRoute(new { controller = "Login", action = "Index" }),
                    };
                }
            }

            if (!ModelState.IsValid)
            {
                vm.Password = "";
                return View(vm);
            }

            UserDto? userDto = await _userService.LoginAsync(new LoginDto()
            {
                Password = vm.Password,
                UserName = vm.UserName
            });

            if (userDto != null)
            {
                UserViewModel userVm = new()
                {
                    Email = userDto.Email,
                    Id = userDto.Id,
                    LastName = userDto.LastName,
                    Name = userDto.Name,
                    Role = userDto.Role,
                    UserName = userDto.UserName,
                    Phone = userDto.Phone,
                    ProfileImage = userDto.ProfileImage,
                };

                HttpContext.Session.Set<UserViewModel>("User", userVm);

                if (userVm.Role == (int)Role.ADMIN)
                {
                    return RedirectToRoute(new { controller = "Home", action = "Index" });
                }

                return RedirectToRoute(new { controller = "InvestorHome", action = "Index" });

            }
            else
            {
                ModelState.AddModelError("userValidation", "Data access is incorrect");
            }

            vm.Password = "";
            return View(vm);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User");
            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }
        public IActionResult Register()
        {
            return View(new RegisterUserViewModel()
            {
                ConfirmPassword = "",
                Email = "",
                LastName = "",
                Name = "",
                Password = "",
                UserName = "",
            });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            SaveUserDto dto = new()
            {
                Id = 0,
                Name = vm.Name,
                Email = vm.Email,
                UserName = vm.UserName,
                LastName = vm.LastName,
                Password = vm.Password,
                Role = (int)Role.INVESTOR,
                Phone = vm.Phone
            };
            UserDto? returnUser = await _userService.AddAsync(dto);
            if (returnUser != null && returnUser.Id != 0)
            {
                dto.Id = returnUser.Id;
                dto.ProfileImage = FileManager.Upload(vm.ProfileImageFile, dto.Id, "Users");
                await _userService.UpdateAsync(dto);
            }

            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }

        public IActionResult AccessDenied()
        {
            if (_userSession.HasUser())
            {
                return View();
            }

            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }
    }
}
