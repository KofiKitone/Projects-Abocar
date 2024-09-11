using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Abocar.Data;
using Abocar.Models;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace Abocar.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id, string data)
        {
            switch (data)
            {
                case "FromAdmin":
                    TempData["FromAdmin"] = "Yes";
                    break;
                case "FromOrder":
                    TempData["isFromOrder"] = "Yes";
                    break;
                case "FromMessage":
                    TempData["FromMessage"] = "Yes";
                    break;
            }
            ViewData["Categories"] = new SelectList(_context.SubCategories, "Id", "Name");
            if (id == null)
            {
                return NotFound();
            }
            ProductDetailsVM product = new ProductDetailsVM
            {
                Product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id),
                Image = await _context.ProductImages.Where(x => x.ProductId == id).ToListAsync(),
                Review = await _context.Review.Where(x => x.ProductId == id).OrderBy(x => x.CreatedAt).ToListAsync()
            };
            if (product.Review != null && product.Review.Count() != 0)
            {
                int averageRating = 0;
                int ratingCount = product.Review.Count();

                foreach (var item in product.Review)
                {
                    averageRating += item.Rating;
                }

                TempData["AverageRating"] = averageRating / ratingCount;
                TempData["NumberOfRating"] = ratingCount + " Confirmed Reviews";
            }
            else if (product.Review != null)
            {
                TempData["NumberOfRating"] = product.Review.Count() + " Reviews";
            }
            else
            {
                TempData["NumberOfRating"] = "0 Reviews"; // Handle the case where product.Review is null
            }

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Administrator, Vendor")]
        public async Task<IActionResult> Create()
        {
            ViewData["Brands"] = await _context.Brand.ToListAsync();
            //ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name");
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,FullDestription, Description,Price,DiscountedPrice,Brand,CreatedAt,StockQuantity,CategoryId,SubCategoryId,VendorId,Size,Color,Length,Width,Height,weight")] Product product)
        {
            var vendor = await _context.Vendors.Where(x => x.UserId == User.Identity.Name).FirstOrDefaultAsync();
            var subCategory = await _context.SubCategories.Where(x => x.Id == product.SubCategoryId).FirstOrDefaultAsync();

            Product prod = new Product();
            prod.Name = product.Name;
            prod.Description = product.Description;
            prod.Price = product.Price;
            prod.DiscountedPrice = product.DiscountedPrice;
            prod.Brand = product.Brand;
            prod.FullDestription = product.FullDestription;
            prod.CreatedAt = DateTime.Now;
            prod.StockQuantity = product.StockQuantity;
            prod.SubCategoryId = product.SubCategoryId;
            prod.SubCategory = subCategory.Name;
            prod.VendorId = vendor.Id;
            prod.Size = product.Size;
            prod.Color = product.Color;
            prod.Length = product.Length;
            prod.Width = product.Width;
            prod.Height = product.Height;
            prod.Weight = product.Weight;
            prod.MainPrice = prod.DiscountedPrice + prod.commission;
            prod.isActive = false;
            try
            {
                await _context.AddAsync(prod);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Account/Manage/Product", new {area = "Identity"});
            } catch
            {
                ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "Name");
                return View(product);
            }
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Administrator, Vendor")]
        public async Task<IActionResult> Edit(int? id, string data)
        {
            if (data == "AddImage")
            {
                TempData["AddImage"] = "AddImage";
            }
            if (id == null)
            {
                return NotFound();
            }
            ViewData["PImages"] = await _context.ProductImages.ToListAsync();
            var product = await _context.Products.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound();
            }
            ProductSet productSet = new ProductSet
            { Product = product };

            ViewData["CategoryId"] = new SelectList(_context.SubCategories, "Id", "Name", product.SubCategoryId);
            return View(productSet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string FullDes, [Bind("Id,Name,Description,FullDestription,Price,DiscountedPrice,Brand,CreatedAt,StockQuantity,CategoryId,SubCategoryId,VendorId,Size,Color,Length,Width,Height,weight")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            var prod = await _context.Products.FindAsync(id);
            var subCategory = await _context.SubCategories.Where(x => x.Id == product.SubCategoryId).FirstOrDefaultAsync();
            prod.Name = product.Name;
            prod.Description = product.Description;
            prod.FullDestription = product.FullDestription;
            prod.Price = product.Price;
            prod.DiscountedPrice = product.DiscountedPrice;
            prod.Brand = product.Brand;
            prod.FullDestription = product.FullDestription;
            prod.CreatedAt = product.CreatedAt;
            prod.StockQuantity = product.StockQuantity;
            prod.SubCategoryId = product.SubCategoryId;
            prod.SubCategory = subCategory.Name;
            prod.Size = product.Size;
            prod.Color = product.Color;
            prod.Length = product.Length;
            prod.Width = product.Width;
            prod.Height = product.Height;
            prod.Weight = product.Weight;
            prod.isActive = false;

            product.FullDestription = FullDes;
            try
            {
                _context.Update(prod);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToPage("/Account/Manage/Product", new { area = "Identity" });
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Administrator, Vendor")]
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["Categories"] = new SelectList(_context.SubCategories, "Id", "Name");
            if (id == null) { return NotFound(); }
            ProductDetailsVM product = new ProductDetailsVM
            {
                Product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id),
                Image = await _context.ProductImages.Where(x => x.ProductId == id).ToListAsync(),
                Review = await _context.Review.Where(x => x.ProductId == id).OrderBy(x => x.CreatedAt).ToListAsync()
            };
            if (product.Review != null && product.Review.Count() != 0)
            {
                int averageRating = 0;
                int ratingCount = product.Review.Count();

                foreach (var item in product.Review)
                {
                    averageRating += item.Rating;
                }

                TempData["AverageRating"] = averageRating / ratingCount;
                TempData["NumberOfRating"] = ratingCount + " Confirmed Reviews";
            }
            else if (product.Review != null)
            {
                TempData["NumberOfRating"] = product.Review.Count() + " Reviews";
            }
            else
            {
                TempData["NumberOfRating"] = "0 Reviews"; // Handle the case where product.Review is null
            }
            if (product == null) { return NotFound(); }
            ViewData["Vendor"] = await _context.Vendors.Where(x => x.Id == product.Product.VendorId).FirstOrDefaultAsync();
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            await _context.SaveChangesAsync();
            return RedirectToPage("/Account/Manage/Product", new { area = "Identity" });
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await DeleteProductImages(product.Id);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product and its related images removed successfully";
            return RedirectToPage("/Account/Manage/Product", new { area = "Identity" });
        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }


        //adding to cart
        [Authorize]
        public async Task<IActionResult> AddToCart(int Id, string Quantity)
        {
            bool isFromHome = false;
            if (Quantity == "0" || Quantity == null)
            {
                Quantity = "1"; isFromHome = true;
            } 
            else
            {
                Quantity = "1"; isFromHome = false;
            }
            //checking if Id exist
            if (Id == 0)
            {
                return NotFound();
            }
            //fetching and checking if user exist
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }
            //checking if product already exist in cart
            var exist = await _context.CartItems.Where(x => x.ProductId == Id && x.UserId == user.Id).ToListAsync();
            if (exist.Count() != 0)
            {
                TempData["SuccessMessage"] = "Product already exist in your cart";
                if (isFromHome == true)
                {
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Details", "Products", new { id = Id });
            }

            //fetching and checking if product exist
            var product = await _context.Products.FindAsync(Id);
            if (product == null)
            {
                return NotFound();
            }
            //fetching and checking if productImages exist  
            var productImages = await _context.ProductImages.Where(x => x.ProductId == Id).ToListAsync();
            //confimation check for product and productImages 
            if (productImages == null || !productImages.Any()){ return NotFound(); }
            
            DateTime date = DateTime.Now;
            CartItem cart = new CartItem();

            cart.UserId = user.Id;
            cart.DateCreated = DateTime.Now.ToString();
            cart.ProductId = product.Id;
            cart.ImageId = productImages.First().Id;
            cart.Quantity = int.Parse(Quantity);
            cart.Total = cart.Quantity * Convert.ToDouble(product.MainPrice);
 
            if (cart.ProductId != 0 && cart.ImageId != 0)
            {
                await _context.CartItems.AddAsync(cart);
                await _context.SaveChangesAsync();
                if (isFromHome == true)
                {
                    TempData["SuccessMessage"] = "Item add successfully";
                    return RedirectToAction("Index", "Home");
                }
                TempData["SuccessMessage"] = "Item add successfully";
                return RedirectToAction("Details", "Products", new { id = Id });
            }
            return BadRequest();
        }

        //removing from cart 
        public async Task<IActionResult> DeleteFromCart(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                //success message 
                return RedirectToPage("/Account/Manage/Product", new { area = "Identity" });
            }
            //Implement error message 
            return RedirectToPage("/Account/Manage/Product", new { area = "Identity" });;

        }

        //add reveiw 
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(string ReviewText, int Rating, int Id)
        {
            if (ReviewText != null)
            {
                var user = await _userManager.FindByEmailAsync(User.Identity?.Name);
                var review = new Review();
                review.ReviewerName = user.FirstName + " " + user.LastName; 
                review.ReviewText = ReviewText;
                review.Rating = Rating;
                review.ReviewId = Guid.NewGuid();
                review.ProductId = Id;
                review.UserId = Guid.Parse(user.Id);
                review.CreatedAt = DateTime.Now;
                review.IsVerified = true;
                review.Upvotes = 0;
                review.Downvotes = 0;
                if (Rating == 0){ review.Downvotes = 1; } 
                    else if (Rating >= 1) { review.Upvotes = 1; };
                await _context.Review.AddAsync(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Products", new { id = Id });
            }
            return RedirectToAction("Details", "Products", new { id = Id });
        }

        [Authorize]
        public async Task<IActionResult> AddToWishlist(int Id)
        {
            if (Id != 0)
            {
                var wish = await _context.Wishlist.Where(x => x.Product == Id).FirstOrDefaultAsync();
                if (wish == null)
                {
                    var user = await _userManager.FindByEmailAsync(User.Identity?.Name);
                    var wishlist = new Wishlist();
                    wishlist.Product = Id;
                    wishlist.UserId = user.Id;
                    wishlist.CreatedAt = DateTime.Now;

                    await _context.Wishlist.AddAsync(wishlist);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Item add successfully";
                    return RedirectToPage("/Account/Manage/Cart");
                }
                else
                {
                    TempData["SuccessMessage"] = "Item already exist in Wishlist";
                    return RedirectToPage("/Account/Manage/Cart");
                }
                
            }else
            {
                return RedirectToAction("Homepage", "Home");
            }
            
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ActivateProduct (int Id)
        {
            decimal price = 0;
            decimal commission = 0;
            double percentage = 0.15;
            if (Id == 0) { return NotFound("Product Not Found"); }
            var product = await _context.Products.Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (product == null) { return NotFound("Product Not Found"); }
            price = product.DiscountedPrice;
            commission = Convert.ToDecimal(percentage) * price;
            product.commission = commission;
            product.MainPrice = Convert.ToDecimal(product.commission) + product.DiscountedPrice;
            product.isActive = true;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Account/Manage/Product", new { area = "Identity" });
        }


        [HttpGet("/User/GetUserDetails/{id}")]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["YesUser"] = user;

            return PartialView("_userView");
        }

        public async Task<IActionResult> DeleteProductImage(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Where(x => x.Id == productImage.ProductId).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound();
            }

            // Delete the image file from the file system
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, productImage.ImagePath);
            if (System.IO.File.Exists(imagePath))
            {
                try
                {
                    System.IO.File.Delete(imagePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting image file: {0}", ex.Message);
                    // Handle error (optional)
                }
            }

            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "Products", new { id = product.Id, data = "AddImage" });
        }

        public async Task DeleteProductImages(int id)
        {
            if (id != 0)
            {
                var productImages = await _context.ProductImages.Where(x => x.ProductId == id).ToListAsync();

                if (productImages != null)
                {
                    foreach (var image in productImages)
                    {
                        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImagePath);
                        if (System.IO.File.Exists(imagePath))
                        {
                            try
                            {
                                System.IO.File.Delete(imagePath);
                            }
                            catch (Exception ex)
                            {
                                // Handle the exception (e.g., log it)
                            }
                        }

                        _context.ProductImages.Remove(image);
                    }

                    await _context.SaveChangesAsync();
                }
            }
        }


    }

}
