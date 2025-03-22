using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Migrations;
using TodoApp.Models;
using TodoApp.Models.ViewModel;
using EditUserViewModel = TodoApp.Models.ViewModel.EditUserViewModel;


namespace TodoApp.Areas.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Super Admin")]

    public class HomeController : Controller
    {

        private readonly UserManager<IdentityUser> _UserManger;
        private readonly RoleManager<IdentityRole> _RoleManager;



        public HomeController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._UserManger = userManager;
            this._RoleManager = roleManager;

        }
        public IActionResult Index()
        {
            var users = _UserManger.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("User not found.");
            }

            var user = await _UserManger.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var appUser = user as ApplicationUser;
            if (appUser == null)
            {
                return BadRequest("Invalid user type.");
            }
            var userroles = await _UserManger.GetRolesAsync(user);
            var allRoles = _RoleManager.Roles.Select(r => r.Name).ToList();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Name = appUser.Name,
                Address = appUser.Adress,
                Role = userroles.FirstOrDefault(),
                AllRoles = allRoles


            };



            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var allRoles = _RoleManager.Roles?.Select(r => r.Name).ToList() ?? new List<string>();
                return View(model);
            }

            var user = await _UserManger.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var appUser = user as ApplicationUser;
            if (appUser == null)
            {
                return BadRequest("Invalid user type.");
            }
            appUser.Name = model.Name;
            appUser.Adress = model.Address;
            var updateResult = await _UserManger.UpdateAsync(appUser);
            if (!updateResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to update user details.");
                return View(model);
            }
            var userRoles = await _UserManger.GetRolesAsync(user);
            if (!userRoles.Contains(model.Role))
            {
                await _UserManger.RemoveFromRolesAsync(user, userRoles);
                await _UserManger.AddToRoleAsync(user, model.Role);
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _UserManger.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest("Invalid user type.");

            }
            var currentUserId = _UserManger.GetUserId(User);

            if (user.Id == currentUserId)
            {
                return BadRequest("You cannot delete your own account while logged in.");
            }
            var delete = await _UserManger.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            var allRoles = _RoleManager.Roles?.Select(r => r.Name).ToList() ?? new List<string>();
            var data = new EditUserViewModel
            {
                AllRoles = allRoles
            };

            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllRoles = await _RoleManager.Roles.Select(r => r.Name).ToListAsync();
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var existingUser = await _UserManger.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "A user with this email already exists.");
                    model.AllRoles = await _RoleManager.Roles.Select(r => r.Name).ToListAsync();
                    return View(model);
                }
                var user = new ApplicationUser
                {
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                    EmailConfirmed=true,

                };
                string defaultPassword = "Default@123";

                var result = await _UserManger.CreateAsync(user, defaultPassword);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Password is (Default@123)";
                    return RedirectToAction("Index");

                }
            }
            model.AllRoles ??= new List<string>();
            return View(model);


        }

    }
}
