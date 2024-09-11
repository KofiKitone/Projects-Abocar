using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Abocar.Data;
using Abocar.Models;
using Microsoft.CodeAnalysis.Operations;

namespace Abocar.Controllers
{
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SubCategories/Create
        public IActionResult Create(string data)
        {
            if (data == null) { return NotFound(); }
            if (data == "subCategory")
            {
                TempData["subCategory"] = "yes";
            }
            else if (data == "brand")
            {
                TempData["brand"] = "yes";
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,CategoryId")] SubCategory subCategory)
        {
            int Id = Convert.ToInt32(TempData["Id"]);
            Category category = await _context.Category.Where(x => x.Id == Id).FirstOrDefaultAsync();

            SubCategory subcategory = new SubCategory
            {
                Description = subCategory.Description,
                Name = subCategory.Name,
            };
            _context.Add(subcategory);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Item created successfully";
            return RedirectToPage("/Account/Manage/Options", new { area = "Identity" });

        }
        
        // GET: SubCategories/Edit/5
        public async Task<IActionResult> Edit(int? id, string data)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (data == null) { return NotFound(); }
            switch (data)
            {
                case "brand":
                    var brand = await _context.Brand.FindAsync(id);
                    if (brand != null)
                    {
                        var BrandandCat = new BrandandCat
                        {
                            brand = brand,
                            active = "brand"
                        };
                        ViewData["BandandCat"] = BrandandCat;
                        break;
                    }
                    break;
                case "subCategory":
                    var subCategor = await _context.SubCategories.FindAsync(id);
                    if (subCategor != null)
                    {
                        var BrandandCat = new BrandandCat
                        {
                            subCategory = subCategor,
                            active = "subCategory"
                        };
                        ViewData["BandandCat"] = BrandandCat;
                        break;
                    }
                    break;
            }

            var subCategory = await _context.SubCategories.FindAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }
            return View(subCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] SubCategory subCategory)
        {
            if (id != subCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubCategoryExists(subCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Item updated successfully";
                return RedirectToPage("/Account/Manage/Options", new { area = "Identity" });
            }
            return View(subCategory);
        }

        public async Task<IActionResult> CreateBrand(string name)
        {
            if (name != null)
            {
                var brand = new Brand
                {
                    Name = name,
                    CreatedAt = DateTime.Now
                };
                await _context.Brand.AddAsync(brand);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Account/Manage/Options", new { area = "Identity" });
            }
            return RedirectToAction("Create", "SubCatgories", new { data = "brand" });
        }
        public async Task<IActionResult> EditBrand(int id, string name)
        {
            if (id != 0 && name != "")
            {
                var brand = await _context.Brand.FindAsync(id);
                if (brand != null)
                {
                    brand.Name = name;
                    _context.Brand.Update(brand);
                    await _context.SaveChangesAsync();
                }
                return RedirectToPage("/Account/Manage/Options", new { area = "Identity" });
            }
            return RedirectToAction("Edit", "Subcategories", new { id = id }); 
        }

        // GET: SubCategories/Delete/5
        private bool SubCategoryExists(int id)
        {
            return _context.SubCategories.Any(e => e.Id == id);
        }
    }
}
