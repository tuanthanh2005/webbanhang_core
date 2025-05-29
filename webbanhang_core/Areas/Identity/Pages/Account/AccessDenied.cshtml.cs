using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webbanhang_core.Models;
using System.Threading.Tasks;

namespace webbanhang_core.Areas.Identity.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AccessDeniedModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string Email, string Password)
        {
            var result = await _signInManager.PasswordSignInAsync(Email, Password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(Email);
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "AdminDashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản không có quyền Admin.");
                    return Page();
                }
            }

            ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không đúng.");
            return Page();
        }
    }
}
