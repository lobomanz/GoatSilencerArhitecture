using GoatSilencerArchitecture.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Web;

namespace GoatSilencerArchitecture.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var userList = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserWithRolesViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    Roles = roles.ToList()
                });
            }

            return View(userList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return RedirectToAction("Index");
        }

        private void PopulateRoles()
        {
            ViewBag.Roles = _roleManager.Roles.ToList();
        }

        public IActionResult Create()
        {
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _roleManager.Roles.ToList();
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                DisplayName = model.DisplayName
            };
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                ViewBag.Roles = _roleManager.Roles.ToList();
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, model.Role);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));


            var link = Url.Action(
                "ResetPassword",
                "Account",
                new { area = "Admin", email = user.Email, token = encodedToken },
                protocol: Request.Scheme
            );


            ViewBag.PasswordLink = link;
            ViewBag.CreatedUserEmail = user.Email;
            return View("UserCreated");
        }
    }
}
