using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: Users
        public IActionResult Index()
        {
            ViewBag._userManager = _userManager;
            return View(_userManager.Users.ToList());
        }

        public IActionResult AddAdminRole(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user != null)
            {
                var result = _userManager.AddToRoleAsync(user, "Admin").Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to add admin role");
                }
            }
            return View();
        }

        public IActionResult RemoveAdminRole(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user != null)
            {
                // Check if the user is not the login onr
                if (user.UserName == User.Identity.Name)
                {
                    ModelState.AddModelError("", "You cannot remove admin role from yourself");
                    return View();
                }
                var result = _userManager.RemoveFromRoleAsync(user, "Admin").Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to remove admin role");
                }
            }
            return View();
        }
    }
}
