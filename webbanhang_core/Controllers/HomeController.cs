using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webbanhang_core.Models;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
namespace webbanhang_core.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {

            var pageSize = 3;
            var dsSanPham = _db.Products.ToList();
            return View(dsSanPham.Skip((pageSize - 3) * pageSize).Take(pageSize).ToList());
        }
        public IActionResult LoadMore(int page = 1)
        {
            var pageSize = 4;
            var dsSanPham = _db.Products.ToList();
            return PartialView("_ProductPartial", dsSanPham.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }
        public IActionResult Privacy()
        {
            return View();
        }
     
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
