using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webbanhang_core.Models;

namespace webbanhang_core.Controllers
{
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
            var totalItems = _db.products.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var dsSanPham = _db.products
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
            var sp = _db.products.Find(id);
            if (sp == null) return NotFound();
            return View(sp);
        }

        // Xóa sản phẩm - POST
        [HttpPost]
        [ActionName("DeleteConfirm")]
        public IActionResult DeleteConfirm(int id)
        {
            var product = _db.products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            // Xóa hình cũ
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var oldFilePath = Path.Combine(_hosting.WebRootPath, product.ImageUrl);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            _db.products.Remove(product);
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

            _db.products.Add(p);
            _db.SaveChanges();
            TempData["success"] = "Đã thêm sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        // Giao diện cập nhật sản phẩm
        public IActionResult Update(int id)
        {
            var product = _db.products.Find(id);
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
            var existingProduct = _db.products.Find(product.Id);
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
            var path = Path.Combine(_hosting.WebRootPath, "images/products");

            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var saveFile = Path.Combine(path, filename);
            using (var filestream = new FileStream(saveFile, FileMode.Create))
            {
                image.CopyTo(filestream);
            }

            return "images/products/" + filename;
        }
    }
}
