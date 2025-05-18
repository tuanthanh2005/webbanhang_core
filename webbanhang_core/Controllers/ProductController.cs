using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webbanhang_core.Models;

namespace webbanhang_core.Controllers
{
    public class ProductController : Controller
    {
            /*
             *các tác vụ
             -liet ke san pham
             -themsp
             -sua sp
             - xoa spq
             Tạo ?? action:
                + index: tra ve giao dien liet ke ds sp
                + add: tra ve giao dien them moi san pham

             */
            private ApplicationDbContext _db;
        public ProductController(ApplicationDbContext db) 
        {
            _db = db;
        }

        public IActionResult Index()
        {
            // doc tat ca cac san pham trong csdl
            var dsSanPham = _db.products.Include(x=>x.Category).ToList();
            return View(dsSanPham);
        }
        // xóa sp
        public IActionResult Delete(int id) {
            var sp = _db.products.Find(id);
            return View(sp);
        }
        public IActionResult DeleteConfirm(int id)
        {
            //truy van san pham can xoa trong csdl
            //var sp = _db.products.Where(x=>x.Id == id).FirstOrDefault(); cach 1
            var sp = _db.products.Find(id);
            //thuc hien xoa 
            _db.products.Remove(sp);
            _db.SaveChanges();
            // thông báo t and f neu muon
            // dieu hướng
            return RedirectToAction("Index");
        }
    }
}
