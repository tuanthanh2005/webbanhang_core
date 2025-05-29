using System.Net.Sockets;
using Microsoft.AspNetCore.Authorization; // 👈 Thêm namespace
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webbanhang_core.Models;

namespace webbanhang_core.Controllers
{
    [Authorize(Roles = "Admin")] // 👈 Chỉ Admin được truy cập toàn bộ controller
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hosting;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment hosting)
        {
            _db = db;
            _hosting = hosting;
        }

        // Liệt kê danh sách sản phẩm
        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            var totalItems = _db.Products.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var dsSanPham = _db.Products
                .Include(x => x.Category)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(dsSanPham);
        }

        // Xóa sản phẩm - GET
        public IActionResult Delete(int id)
        {
            var sp = _db.Products.Find(id);
            if (sp == null) return NotFound();
            return View(sp);
        }

        // Xóa sản phẩm - POST
        [HttpPost]
        [ActionName("DeleteConfirm")]
        public IActionResult DeleteConfirm(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var oldFilePath = Path.Combine(_hosting.WebRootPath, product.ImageUrl);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            _db.Products.Remove(product);
            _db.SaveChanges();
            TempData["success"] = "Product deleted successfully!";
            return RedirectToAction("Index");
        }

        // Giao diện thêm mới sản phẩm
        public IActionResult Add()
        {
            ViewBag.TLoai = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();

            return View();
        }

        // Xử lý thêm mới sản phẩm
        [HttpPost]
        public IActionResult Add(Product p, IFormFile ImageUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TLoai = _db.Categories.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();
                return View(p);
            }

            if (ImageUrl != null)
            {
                p.ImageUrl = SaveImage(ImageUrl);
            }

            _db.Products.Add(p);
            _db.SaveChanges();
            TempData["success"] = "Đã thêm sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        // Giao diện cập nhật sản phẩm
        public IActionResult Update(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null) return NotFound();

            ViewBag.TLoai = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();

            return View(product);
        }

        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public IActionResult Update(Product product, IFormFile ImageUrl)
        {
            var existingProduct = _db.Products.Find(product.Id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            if (ImageUrl != null)
            {
                product.ImageUrl = SaveImage(ImageUrl);

                if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_hosting.WebRootPath, existingProduct.ImageUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }
            else
            {
                product.ImageUrl = existingProduct.ImageUrl;
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.ImageUrl = product.ImageUrl;

            _db.SaveChanges();
            TempData["success"] = "Product updated successfully!";
            return RedirectToAction("Index");
        }

        // Lưu ảnh sản phẩm
        private string SaveImage(IFormFile image)
        {
            var filename = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var path = Path.Combine(_hosting.WebRootPath, "images/Products");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var saveFile = Path.Combine(path, filename);
            using (var filestream = new FileStream(saveFile, FileMode.Create))
            {
                image.CopyTo(filestream);
            }

            return "images/Products/" + filename;
        }
    }
}
