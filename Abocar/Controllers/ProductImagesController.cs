using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Abocar.Data;
using Abocar.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace Abocar.Controllers
{
    public class ProductImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductImagesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: ProductImages
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProductImages.ToListAsync());
        }

        // GET: ProductImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductImage Image, int Id)
        {
            if (Image.FormFile == null || Image.FormFile.Length == 0)
            {
                throw new ArgumentException("No image file uploaded.");
            }

            // Validate file size (optional)
            if (Image.FormFile.Length > 5242880) // 1 MB limit (adjust as needed)
            {
                throw new ArgumentException("Image file size exceeds the limit.");
            }

            // Validate file type (optional)
            string validExtensions = ".jpg,.jpeg,.png"; // Allowed extensions
            if (!validExtensions.Contains(Path.GetExtension(Image.FormFile.FileName).ToLower()))
            {
                throw new ArgumentException("Invalid image file type.");
            }

            // Generate unique filename
            string fileName = Path.GetFileNameWithoutExtension(Image.FormFile.FileName);
            string extension = Path.GetExtension(Image.FormFile.FileName);
            fileName = Image.Name + DateTime.Now.ToString("yymmssfff") + extension;

            // Construct image path (use a separate variable)
            string savedImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "ProductPictures", fileName);

            // Save the file with error handling
            try
            {
                using (var fileStream = new FileStream(savedImagePath, FileMode.Create))
                {
                    await Image.FormFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving image file: " + ex.Message);
            }

            // Update user's image path and return the saved path
            ProductImage productImage = new ProductImage
            {
                Name = Image.Name,
                ImagePath = "ProductPictures/" + fileName,
                ProductId = Id
            };
            Image.ImagePath = "ProductPictures/" + fileName;
            Image.ProductId = Id;
            try
            {
                await _context.AddAsync(Image);
                await _context.SaveChangesAsync();
                // Success message or logic
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error saving product image: {Message}", ex.Message);
            }
            return RedirectToAction("Edit", "Products", new {Id = Id, data="AddImage"});
        }

        // GET: ProductImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }
            return View(productImage);
        }

        // POST: ProductImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImagePath,ProductId")] ProductImage productImage)
        {
            if (id != productImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductImageExists(productImage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productImage);
        }

        // GET: ProductImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // POST: ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage != null)
            {
                _context.ProductImages.Remove(productImage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductImageExists(int id)
        {
            return _context.ProductImages.Any(e => e.Id == id);
        }



        public async Task<string> SaveImage(ProductImage Image, int Id)
        {
            if (Image.FormFile == null || Image.FormFile.Length == 0)
            {
                throw new ArgumentException("No image file uploaded.");
            }

            // Validate file size (optional)
            if (Image.FormFile.Length > 5242880) // 1 MB limit (adjust as needed)
            {
                throw new ArgumentException("Image file size exceeds the limit.");
            }

            // Validate file type (optional)
            string validExtensions = ".jpg,.jpeg,.png"; // Allowed extensions
            if (!validExtensions.Contains(Path.GetExtension(Image.FormFile.FileName).ToLower()))
            {
                throw new ArgumentException("Invalid image file type.");
            }

            // Generate unique filename
            string fileName = Path.GetFileNameWithoutExtension(Image.FormFile.FileName);
            string extension = Path.GetExtension(Image.FormFile.FileName);
            fileName = Image.Name + DateTime.Now.ToString("yymmssfff") + extension;

            // Construct image path (use a separate variable)
            string savedImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "ProductPictures", fileName);

            // Save the file with error handling
            try
            {
                using (var fileStream = new FileStream(savedImagePath, FileMode.Create))
                {
                    await Image.FormFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving image file: " + ex.Message);
            }

            // Update user's image path and return the saved path
            ProductImage productImage = new ProductImage
            {
                Name = Image.Name,
                ImagePath = "ProductPictures/" + fileName,
                ProductId = Id
            };
            Image.ImagePath = "ProductPictures/" + fileName;
            Image.ProductId = Id;
            try
            {
                await _context.AddAsync(Image);
                await _context.SaveChangesAsync();
                // Success message or logic
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error saving product image: {Message}", ex.Message);
            }
            return "";
        }
    }
}
