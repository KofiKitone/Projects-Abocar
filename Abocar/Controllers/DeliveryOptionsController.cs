using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Abocar.Data;
using Abocar.Models;
using Microsoft.AspNetCore.Authorization;

namespace Abocar.Controllers
{
    public class DeliveryOptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeliveryOptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DeliveryOptions
        public async Task<IActionResult> Index()
        {
            var deliveryOptions = await _context.DeliveryOption.ToListAsync();
            var paymentOptions = await _context.PaymentOption.ToListAsync();
            ViewData["DeliveryOptions"] = deliveryOptions; ViewData["PaymentOptions"] = paymentOptions;
            return View();
        }

        // GET: DeliveryOptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryOption = await _context.DeliveryOption
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deliveryOption == null)
            {
                return NotFound();
            }

            return View(deliveryOption);
        }

        // GET: DeliveryOptions/Create
        public IActionResult Create(string data)
        {
            if (data == "Payment")
            {
                TempData["DPChoice"] = "Payment";
            }else if (data == "Delivery")
            {
                TempData["DPChoice"] = "Delivery";
            }else
            {
                TempData["DPChoice"] = "PickUps";
            }
            return View();
        }

        // POST: DeliveryOptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string data, EditOptionViewModel viewModel)
        {
            try 
            {
                switch (data)
                {
                    case "Delivery":
                        await _context.DeliveryOption.AddAsync(viewModel.DeliveryOption);
                        await _context.SaveChangesAsync();
                        break; 
                    case "Payment":
                        await _context.PaymentOption.AddAsync(viewModel.PaymentOption);
                        await _context.SaveChangesAsync();
                        break;
                    case "LocalPickUp":
                        await _context.LocalPickUp.AddAsync(viewModel.LocalPickUp);
                        await _context.SaveChangesAsync();     
                        break; 
                }
                return RedirectToPage("/Account/Manage/Options", new { area = "Identity" });
            }catch 
            {
                return View(viewModel);
            }
        }

        

        // GET: DeliveryOptions/Edit/5
        public async Task<IActionResult> Edit(int? id, string data)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = new EditOptionViewModel();
            viewModel.DPChoice = data;

            if (data == "Payment")
            {
                var paymentOption = await _context.PaymentOption.FindAsync(id);
                if (paymentOption == null)
                {
                    return NotFound();
                }
                viewModel.PaymentOption = paymentOption;
            }
            else if (data == "Delivery")
            {
                var deliveryOption = await _context.DeliveryOption.FindAsync(id);
                if (deliveryOption == null)
                {
                    return NotFound();
                }
                viewModel.DeliveryOption = deliveryOption;
            }
            else if (data == "PickUps")
            {
                var localPickUp = await _context.LocalPickUp.FindAsync(id);
                if (localPickUp == null)
                {
                    return NotFound();
                }
                viewModel.LocalPickUp = localPickUp;
            }

            return View(viewModel);
        }


        // POST: DeliveryOptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string data, EditOptionViewModel viewModel)
        {
            if (id == 0)
            {
                return NotFound();
            }

            try 
            {
                switch (data)
                {
                    case "Delivery":
                        var delivery = await _context.DeliveryOption.FindAsync(id);
                        if (delivery != null)
                        {
                            delivery.Name = viewModel.DeliveryOption.Name;
                            delivery.Description = viewModel.DeliveryOption.Description;
                            delivery.Cost = viewModel.DeliveryOption.Cost;
                            delivery.isActive = viewModel.DeliveryOption.isActive;
                            _context.DeliveryOption.Update(delivery);
                            await _context.SaveChangesAsync();
                        }
                        break; 
                    case "Payment":
                        var payment = await _context.PaymentOption.FindAsync(id);
                        if (payment != null)
                        {
                            payment.Name = viewModel.PaymentOption.Name;
                            payment.Description = viewModel.PaymentOption.Description;
                            payment.isActive = viewModel.PaymentOption.isActive;
                            _context.PaymentOption.Update(payment);
                            await _context.SaveChangesAsync();
                        }
                        break;
                    case "LocalPickUp":
                        var loc = await _context.LocalPickUp.FindAsync(id);
                        if (loc != null)
                        {
                            loc.Name = viewModel.LocalPickUp.Name;
                            loc.AddressLine = viewModel.LocalPickUp.AddressLine;
                            loc.isActive = viewModel.LocalPickUp.isActive;
                            _context.LocalPickUp.Update(loc);
                            await _context.SaveChangesAsync();
                        }
                         break; 
                }
                return RedirectToPage("/Account/Manage/Options", new { area = "Identity" });
            }catch 
            {
                return View(viewModel);
            }
            
        }

        public async Task<IActionResult> EditLocalPickUp(int id, string Name, string AddressLine, bool isActive)
        {
            var loc = await _context.LocalPickUp.FindAsync(id);
            if (loc == null) { return NotFound(); }
            loc.Name = Name;
            loc.AddressLine = AddressLine;
            loc.isActive = isActive;
            _context.LocalPickUp.Update(loc);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Account/Manage/Options", new { area = "Identity" });
        }


        // GET: DeliveryOptions/Delete/5

        // POST: DeliveryOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOption(int id, string data)
        {
            await FindAndDelete(id, data); // Await the async method
            return RedirectToPage("/Account/Manage/Options", new { area = "Identity" });
        }

        public async Task FindAndDelete(int id, string data)
        {
            if (id != 0 && data != null)
            {
                if (data == "LocalPickUp")
                {
                    var item = await _context.LocalPickUp.FindAsync(id);
                    if (item != null)
                    {
                        _context.LocalPickUp.Remove(item);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (data == "Delivery")
                {
                    var item = await _context.DeliveryOption.FindAsync(id);
                    if (item != null)
                    {
                        _context.DeliveryOption.Remove(item);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (data == "Payment")
                {
                    var item = await _context.PaymentOption.FindAsync(id);
                    if (item != null)
                    {
                        _context.PaymentOption.Remove(item);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }


        private bool DeliveryOptionExists(int id)
        {
            return _context.DeliveryOption.Any(e => e.Id == id);
        }



        // Codes for Payment options 
        //get is already handled up there 
        [HttpPost]
        public async Task<IActionResult> AddPaymentOption(string name, string description, string isActive)
        {
            bool Active;
            if (name == null || description == null)
            {
                return RedirectToAction("Create", "DeliveryOptions", new { data = "Payment" });
            }
            if (isActive == "true") { Active = true; } else { Active = false; }
            var paymentOption = new PaymentOption
            {
                Name = name,
                Description = description,
                isActive = Active,
            };
            await _context.PaymentOption.AddAsync(paymentOption);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "DeliveryOptions"); 
        }


        public async Task<IActionResult> EditPaymetOption (int id, string data, string name, string description, string isActive)
        {
            bool active;
            if (id == 0) { return NotFound(); }
            var paymentOption = await _context.PaymentOption.FindAsync(id);
            paymentOption.Name = name;
            paymentOption.Description = description;
            if (isActive == "on") { active = true; } else { active = false; }
            paymentOption.isActive = active;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "DeliveryOptions");
        }
    }






    

}
