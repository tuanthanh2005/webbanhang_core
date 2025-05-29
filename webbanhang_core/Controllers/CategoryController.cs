using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webbanhang_core.Models;

namespace WebBanHang.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //Hiển thị danh sách loại
        public IActionResult Index()
        {
            // Lấy danh sách category hoặc sản phẩm từ DB
            List<Category> categories = _db.Categories.ToList();

            // Nếu không có dữ liệu, trả về danh sách rỗng để View không bị null
            if (categories == null)
            {
                categories = new List<Category>();
            }

            return View(categories); // Truyền danh sách xuống View
        }

        //Hiển thị form thêm mới
        public IActionResult Add()
        {
            return View();
        }

        // Xử lý thêm mới
        [HttpPost]
        public IActionResult Add(Category category)
        {
            if (ModelState.IsValid) //kiểm tra hợp lệ dữ liệu
            {
                //thêm category vào table
                _db.Categories.Add(category);
                _db.SaveChanges();
                TempData["success"] = "Category được thêm thành công";
                return RedirectToAction("Index");
            }
            return View();
        }

        //Hiển thị form cập nhật chủng loại
        public IActionResult Update(int id)
        {
            var category = _db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // Xử lý cập nhật
        [HttpPost]
        public IActionResult Update(Category category)
        {
            if (ModelState.IsValid) //kiểm tra hợp lệ dữ liệu
            {
                //cập nhật category vào table
                _db.Categories.Update(category);
                _db.SaveChanges();
                TempData["success"] = "Category được cập nhật thành công";
                return RedirectToAction("Index");
            }
            return View();
        }

        //Hiển thị form xác nhận xóa
        public IActionResult Delete(int id)
        {
            var category = _db.Categories.FirstOrDefault(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        // Xử lý xóa
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            if (_db.Products.Where(x => x.CategoryId == category.Id).ToList().Count > 0)
            {
                TempData["error"] = "Đã có sản phẩm theo thể loại này. Không thể xoá";
                return RedirectToAction("Index");
            }
            _db.Categories.Remove(category);
            _db.SaveChanges();
            TempData["error"] = "Category được xóa thành công";
            return RedirectToAction("Index");
        }
    }
}