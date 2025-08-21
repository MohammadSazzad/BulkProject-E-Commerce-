using Bulk.DataAccess.Repository.IRepository;
using Bulk.Models;
using Bulk.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int page = 1, int pageSize = 6)
        {
            var allProducts = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            
            var totalProducts = allProducts.Count;
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            
            var products = allProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.PageSize = pageSize;
            ViewBag.HasNextPage = page < totalPages;
            ViewBag.HasPreviousPage = page > 1;
            
            return View(products);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };

            if (id == null || id == 0)
            {
                // Create
                return View(productVM);
            }
            else
            {
                // Edit
                var existingProduct = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Category");
                if (existingProduct == null)
                {
                    return NotFound();
                }
                productVM.Product = existingProduct;
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["Success"] = "Product created successfully.";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["Success"] = "Product updated successfully.";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            return View(productVM);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? ProductFromDb = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Category");
            if (ProductFromDb == null)
            {
                return NotFound();
            }
            return View(ProductFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Category");
            if (obj == null)
            {
                TempData["Error"] = "Product not found.";
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj.Id);
            _unitOfWork.Save();
            TempData["Warning"] = "Product deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
