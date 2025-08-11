using BulkProject.Data;
using BulkProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkProject.Controllers
{
    public class CategoryController : Controller
    {
        private readonly BulkDbContext _context;

        public CategoryController(BulkDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Category> categories = _context.Categories.ToList();
            if (categories == null || !categories.Any())
            {
                TempData["Error"] = "No categories found.";
                return NotFound();
            }
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["Info"] = "Category created successfully.";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var categoryFromDb = _context.Categories.Find(id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                TempData["Success"] = "Category updated successfully.";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Failed to update category. Please check the details.";
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var categoryFromDb = _context.Categories.Find(id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var categoryFromDb = _context.Categories.Find(id);
            if (categoryFromDb == null)
            {
                TempData["Error"] = "Category not found.";
                return NotFound();
            }
            _context.Categories.Remove(categoryFromDb);
            _context.SaveChanges();
            TempData["Warning"] = "Category deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}