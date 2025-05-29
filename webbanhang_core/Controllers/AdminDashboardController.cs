using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace webbanhang_core.Controllers
{
    [Authorize(Roles = "Admin")] // Chỉ Admin truy cập
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
