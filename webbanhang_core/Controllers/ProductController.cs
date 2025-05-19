using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webbanhang_core.Models;
using static System.Net.Mime.MediaTypeNames;

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
        private readonly IWebHostEnvironment _hosting;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment hosting)
        {
            _db = db;
            _hosting = hosting;
        }

        public IActionResult Index()
        {
            // doc tat ca cac san pham trong csdl
            var dsSanPham = _db.products.Include(x => x.Category).ToList();
            return View(dsSanPham);
        }
        // xóa sp
        public IActionResult Delete(int id)
        {
            var sp = _db.products.Find(id);
            return View(sp);
        }
        public IActionResult DeleteConfirm(int id)
        {
            var product = _db.products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            // xoá hình cũ của sản phẩm
            if (!String.IsNullOrEmpty(product.ImageUrl))
            {
                var oldFilePath = Path.Combine(_hosting.WebRootPath, product.ImageUrl);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            // xoa san pham khoi CSDL
            _db.products.Remove(product);
            _db.SaveChanges();
            TempData["success"] = "Product deleted success";
            //chuyen den action index
            return RedirectToAction("Index");
        }
        /*
            Add
            Edit
         */
        public IActionResult Add()
        {
            ViewBag.TLoai = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();

            return View();
        }
        [HttpPost]
        public IActionResult Add(Product p,IFormFile ImageUrl)
        {
            if (ImageUrl != null)
            {
              p.ImageUrl= SaveImage(ImageUrl);
            }
            _db.products.Add(p);
            _db.SaveChanges();
            TempData["success"] = "Đã Thêm Sản Phẩm Thành Công!!!";
            return RedirectToAction("Index");
        }
        // xử lý uploads ảnh
        private string SaveImage(IFormFile image)
        {
            //đặt lại tên file cần lưu
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            //lay duong dan luu tru wwwroot tren server
            var path = Path.Combine(_hosting.WebRootPath, @"images/products");// trong wwwroot
            var saveFile = Path.Combine(path, filename);
            using (var filestream = new FileStream(saveFile, FileMode.Create))
            {
                image.CopyTo(filestream);
            }
            return @"images/products/" + filename;
        }
        public IActionResult Update(int id)
        {
            // truy vấn cơ sở dữ liệu
            var product = _db.products.Find(id);
            //truyền danh sách thể loại cho View để sinh ra điều khiển DropDownList
            ViewBag.TLoai = _db.Categories.Select(x => new SelectListItem
            {

                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View(product);
        }
        //Xử lý cập nhật sản phẩm
        [HttpPost]
        public IActionResult Update(Product product, IFormFile ImageUrl)
        {
          
                var existingProduct = _db.products.Find(product.Id);
                if (ImageUrl != null)
                {
                    //xu ly upload và lưu ảnh đại diện mới
                    product.ImageUrl = SaveImage(ImageUrl);
                // xóa ảnh cũ
                if (!String.IsNullOrEmpty(existingProduct.ImageUrl))
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
                //cập nhật product vào table Product
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;
                _db.SaveChanges();
                TempData["success"] = "Product updated success";
            return RedirectToAction("Index");
        }
    }
}
