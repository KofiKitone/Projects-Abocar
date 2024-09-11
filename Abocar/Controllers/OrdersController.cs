using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Abocar.Data;
using Abocar.Models;
using Microsoft.CodeAnalysis;
using Humanizer;
using Microsoft.Identity.Client;
using PayStack.Net;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Globalization;
using System.Numerics;

namespace Abocar.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _paystackPublicKey;
        private readonly string _paystackSecretKey;
        private readonly IEmailSender _emailSender;
        private PayStackApi PayStack { get; set; }
        private readonly IConfiguration _myconfiguration;

        public OrdersController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _paystackPublicKey = configuration["PaystackPublicKey"];
            _paystackSecretKey = configuration["PaystackSecretKey"];
            configuration = configuration;
            _emailSender = emailSender;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Order;
            return View(await applicationDbContext.ToListAsync());
        }        
        
        
        // codes for payments and transaction 
        public async Task<IActionResult> Payment(string paymentMethod, string deliveryMethod, string? LocalPickUp)
        {
            TempData["PaymentMethod"] = paymentMethod;
            TempData["DeliveryOption"] = deliveryMethod;
            if (LocalPickUp != null || LocalPickUp != "Select Pickup Location")
            {
                TempData["LocalPickUp"] = LocalPickUp;
            }
            string Token = "sk_test_0641b01e8db707ff15c8027209fdf3f7eb7e2868";
            var user = await _userManager.GetUserAsync(User); var paystackApi = new PayStackApi(Token);

            //getting and calculating the subtotal, shipping and total cost 
            var cartItems = await _context.CartItems.Where(x => x.UserId == user.Id).ToListAsync();
            var products = await _context.Products.ToListAsync();
            decimal grandTotal = 0;
            decimal subTotal = 0;
            foreach (var item in cartItems)
            {
                var prod = products.Where(x => x.Id == item.ProductId).FirstOrDefault();
                subTotal = subTotal + (item.Quantity * Convert.ToDecimal(prod.MainPrice));
            }

            var paymentOption = await _context.PaymentOption.Where(x => x.Id == int.Parse(paymentMethod)).FirstOrDefaultAsync();
            if (paymentOption != null) 
            {
                var deliveryOptioin = await _context.DeliveryOption.Where(x => x.Id == int.Parse(deliveryMethod)).FirstOrDefaultAsync();
                grandTotal = deliveryOptioin.Cost + subTotal;
                if (paymentOption.Name == "Cash On Delivery")
                {
                    var transaction = new Transaction
                    {
                        Amount = grandTotal,
                        Name = user.FirstName + " " + user.LastName,
                        TransactionReference = GenerateRef().ToString(),
                        Email = user.Email,
                        OrderId = GenerateOrderNumber().ToString(),
                    };

                    var address = await _context.Address.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
                    var orderedProducts = new List<OrderProduct>();

                    foreach (var item in cartItems)
                    {
                        var product = await _context.Products.Where(x => x.Id == item.ProductId).FirstOrDefaultAsync();
                        var prod = new OrderProduct()
                        {
                            ProductName = product.Name,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            ProductPrice = Convert.ToDecimal(product.MainPrice),
                            Total = Convert.ToDecimal(item.Total),
                        };
                        orderedProducts.Add(prod);
                    }
                    if (transaction != null)
                    {
                        string DeliveryMethod = "";
                        if (TempData["LocalPickUp"] as string != null && TempData["LocalPickUp"] != "Select Pickup Location")
                        {
                            DeliveryMethod = LocalPickUp;
                        }else
                        {
                            DeliveryMethod = deliveryOptioin.Name;
                        }
                        var order = new Order
                        {
                            UserId = user.Id,
                            OrderDate = DateTime.Now,
                            ShippingCost = deliveryOptioin.Cost,
                            Total = grandTotal,
                            Subtotal = subTotal,
                            PaymentOption = paymentOption.Name,
                            DeliveryOption = DeliveryMethod,
                            OrderProduct = orderedProducts,
                            isPaid = false,
                            OrderStatus = "Pending",
                            AddressId = address.Id,
                            OrderNumber = transaction.OrderId,
                        };
                        _context.Transactions.Update(transaction);
                        await _context.Order.AddAsync(order);
                        if (user != null)
                        {
                            var orderItems = order.OrderProduct.Select(op => new OrderProduct
                            {
                                ProductName = op.ProductName,
                                Quantity = op.Quantity,
                                ProductPrice = op.ProductPrice
                            }).ToList();

                            var subject = "Order Confirmation";
                            var message = GenerateEmailBody(user.FirstName + " " + user.LastName, order.OrderNumber, order.OrderDate, orderItems);

                            await _emailSender.SendEmailAsync(user.Email, subject, message);
                        }
                        await EmptyCartAsync(user);
                        TempData["SuccesMessage"] = " Your order has successfully been placed 👍";
                        await _context.SaveChangesAsync();
                        await sendMessage(user, order.OrderNumber, "Confirmed", "Thank you for your order " + order.OrderNumber + " placed on " + order.OrderDate + ".");
                        return RedirectToAction("/Orders", "Orders", new { area = "Identity" });
                    }
                }
                else
                {
                    var request = new TransactionInitializeRequest
                    {
                        AmountInKobo = Convert.ToInt32(grandTotal * 100), // Paystack uses kobo (smallest currency unit)
                        Email = user.Email,
                        Reference = GenerateRef().ToString(),
                        Currency = "GHS",
                        CallbackUrl = "http://localhost:5090/Orders/Confirmation" // Replace with your callback URL
                    };
                    TempData["Reference"] = request.Reference;
                    var initializeResponse = paystackApi.Transactions.Initialize(request);

                    if (initializeResponse.Status)
                    {
                        var transaction = new Transaction
                        {
                            Amount = Convert.ToDecimal(grandTotal),
                            Name = user.FirstName + " " + user.LastName,
                            TransactionReference = request.Reference,
                            Email = user.Email,
                        };
                        transaction.OrderId = "";
                        await _context.Transactions.AddAsync(transaction);
                        await _context.SaveChangesAsync();
                        return Redirect(initializeResponse.Data.AuthorizationUrl);
                    }
                    else
                    {
                        return View("Error");
                    }
                }
            }
            return View();   
        }

        public async Task<IActionResult> Confirmation(TransactionVerifyResponse response) // Corrected to VerifyTransactionResponse
        {

            int paymentMethodId = Convert.ToInt32(TempData["PaymentMethod"]);
            int deliveryOptionId = Convert.ToInt32(TempData["DeliveryOption"]);
            string LocalPickUp = "";
            if (TempData["LocalPickUp"] as string != null)
            {
                LocalPickUp = TempData["LocalPickUp"] as string;
            }

            string Token = "sk_test_0641b01e8db707ff15c8027209fdf3f7eb7e2868";
            var paystackApi = new PayStackApi(Token);
            var verifyResponse = paystackApi.Transactions.Verify(TempData["Reference"] as string);

            if (verifyResponse.Status && verifyResponse.Data.Status == "success")
            {
                string reference = "";
                if (TempData["Reference"] as string != null) { reference = TempData["Reference"] as string; }
                var paymentOption = await _context.PaymentOption.Where(x => x.Id == paymentMethodId).FirstOrDefaultAsync();
                var deliveryOption = await _context.DeliveryOption.Where(x => x.Id == deliveryOptionId).FirstOrDefaultAsync();

                var transaction = await _context.Transactions.Where(x => x.TransactionReference == reference).FirstOrDefaultAsync();
                var user = await _userManager.GetUserAsync(User);
                var cartItems = await _context.CartItems.Where(x => x.UserId == user.Id).ToListAsync();
                var address = await _context.Address.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
                var orderedProducts = new List<OrderProduct>();
                var products = await _context.Products.ToListAsync();

                decimal grandTotal = 0;
                decimal subTotal = 0;
                foreach (var item in cartItems)
                {
                    var prod = products.Where(x => x.Id == item.ProductId).FirstOrDefault();
                    subTotal = subTotal + (item.Quantity * Convert.ToDecimal(prod.MainPrice));
                }
                grandTotal = deliveryOption.Cost + subTotal;
                foreach (var item in cartItems)
                {
                    var product = await _context.Products.Where(x => x.Id == item.ProductId).FirstOrDefaultAsync();
                    var prod = new OrderProduct()
                    {
                        ProductName = product.Name,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        ProductPrice = Convert.ToDecimal(product.MainPrice),
                        Total = Convert.ToDecimal(item.Total),
                    };
                    orderedProducts.Add(prod);
                }
                if (transaction != null)
                {
                    string DeliveryMethod = "";
                    if (LocalPickUp != null || LocalPickUp != "Select Pickup Location")
                    {
                        DeliveryMethod = LocalPickUp;
                    }
                    else
                    {
                        DeliveryMethod = deliveryOption.Name;
                    }

                    var order = new Order
                    {
                        UserId = user.Id,
                        OrderDate = DateTime.Now,
                        ShippingCost = deliveryOption.Cost,
                        Total = grandTotal,
                        Subtotal = subTotal,
                        PaymentOption = paymentOption.Name,
                        DeliveryOption = DeliveryMethod,
                        OrderProduct = orderedProducts,
                        isPaid = true,
                        AddressId = address.Id,
                        OrderStatus = "Confirmed",
                        EstimatedDeliveryDate = GenDate(deliveryOption.Name),
                        OrderNumber = GenerateOrderNumber().ToString(),
                    };
                    transaction.OrderId = order.OrderNumber;
                    transaction.Status = true;
                    _context.Transactions.Update(transaction);
                    await _context.Order.AddAsync(order);

                    // Reduce the stock quantity
                    foreach (var item in orderedProducts)
                    {
                        var product = await _context.Products.Where(x => x.Id == item.ProductId).FirstOrDefaultAsync();
                        if (product != null)
                        {
                            product.StockQuantity -= item.Quantity;
                            _context.Products.Update(product);
                        }
                    }

                    if (user != null)
                    {
                        var orderItems = order.OrderProduct.Select(op => new OrderProduct
                        {
                            ProductName = op.ProductName,
                            Quantity = op.Quantity,
                            ProductPrice = op.ProductPrice
                        }).ToList();

                        var subject = "Order Confirmed";
                        var message = GenerateEmailBody(user.FirstName + " " + user.LastName, order.OrderNumber, order.OrderDate, orderItems);
                        await _emailSender.SendEmailAsync(user.Email, subject, message);
                    }
                    await EmptyCartAsync(user);
                    TempData["SuccesMessage"] = " Your order has successfully been placed 👍";
                    await _context.SaveChangesAsync();
                    await sendMessage(user, order.OrderNumber, "Confirmed", "Thank you for your order " + order.OrderNumber + " placed on " + order.OrderDate + ".");
                }
                return View();
            }
            else
            {
                TempData["ErrorMessage"] = "An unexpected error occured while placing your order 😢. If you have not received a mail for Order Confirmation, kindly try again later";
                return View();
            }

        }


        //methods 
        //generate reference for transaction
        public static string GenerateRef()
        {
            int minRandom = 1000, maxRandom = 9999;
            var now = DateTime.UtcNow;
            var randomNumber = new Random().Next(minRandom, maxRandom + 1); // Generate random number within range (inclusive)
            return $"Abo-{randomNumber}-{now:yyyyMMddHHmmss}";
        }
        //generate ordernumber for order 
        public static string GenerateOrderNumber ()
        {
            string orderNumber;
            Random random = new Random();
            orderNumber = "Abocar-" + random.Next();
            return orderNumber;
        }
        //generate email-body  (message)
        public static string GenerateEmailBody(string customerName, string orderNumber, DateTime orderDate, List<OrderProduct> orderItems)
        {
            var itemsHtml = string.Empty;
            foreach (var item in orderItems)
            {
                itemsHtml += $@"
                <tr>
                    <td style='padding: 8px; border: 1px solid #ddd;'>{item.ProductName}</td>
                    <td style='padding: 8px; border: 1px solid #ddd;'>{item.Quantity}</td>
                    <td style='padding: 8px; border: 1px solid #ddd;'>${item.ProductPrice}</td>
                </tr>";
            }

            return $@"
            <div style='font-family: Arial, sans-serif;'>
                <h2 style='color: #2E4053;'>Order Confirmation</h2>
                <p>Dear {customerName},</p>
                <p>Thank you for your order #{orderNumber} placed on {orderDate.ToString("MMMM dd, yyyy")}.</p>
                <table style='border-collapse: collapse; width: 100%;'>
                    <thead>
                        <tr style='background-color: #f2f2f2;'>
                            <th style='padding: 8px; border: 1px solid #ddd;'>Product</th>
                            <th style='padding: 8px; border: 1px solid #ddd;'>Quantity</th>
                            <th style='padding: 8px; border: 1px solid #ddd;'>Price</th>
                        </tr>
                    </thead>
                    <tbody>
                        {itemsHtml}
                    </tbody>
                </table>
                <p style='margin-top: 20px;'>We hope you enjoy your purchase! If you have any questions, please contact our customer service.</p>
                <p>Best regards,</p>
                <p>Abocar</p>
            </div>";
        }
        //truncate user's cart after order has been placed 
        public async Task EmptyCartAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var userCartItems = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .ToListAsync();
            if (userCartItems.Any())
            {
                _context.CartItems.RemoveRange(userCartItems);
                await _context.SaveChangesAsync();
            }
        }

        //generating estimated delivery datetime 
        public static DateTime GenDate(string deliveryOption)
        {
            DateTime date = DateTime.Now;
            switch (deliveryOption)
            {
                case "Standard Delivery":
                    date = date.AddDays(3);
                    break;
                case "Express Delivery":
                    date = date.AddDays(1);
                    break;
                case "Same Day Delivery":
                    date = date;
                    break;
                case "Local Pickup":
                    date = date.AddDays(3);
                    break;
                default:
                    throw new ArgumentException("Invalid delivery option");
            }

            return date;
        }
        //generating message for user inbox 
        public async Task sendMessage(ApplicationUser user, string OrderNumber, string Subject, string body)
        {
            var order = await _context.Order.Where(x => x.OrderNumber == OrderNumber).FirstOrDefaultAsync();
            if (user != null)
            {
                string userId = user.Id;
                var products = new List<OrderProduct>();

                if (order != null)
                {
                    var message = new Message
                    {
                        UserId = userId,
                        Subject = Subject,
                        Body = body,
                        OrderId = order.Id,
                        SentAt = DateTime.Now,
                        IsRead = false,
                    };
                    await _context.Messages.AddAsync(message);
                    await _context.SaveChangesAsync();
                }

            }

        }

        //order processing 
        // returning the page 
        public IActionResult OrderPrecessing(string data)
        {
            if (string.IsNullOrEmpty(data)) { return NotFound("Order Not Found"); };
            var order = _context.Order.Where(x => x.OrderNumber == data).Include(x => x.OrderProduct).FirstOrDefault();
            if (order == null) { return NotFound("Order Not Found"); }
            var image = _context.ProductImages.ToList();
            var address = _context.Address.Where(x => x.Id == order.AddressId).FirstOrDefault();
            ViewData["Order"] = order;
            ViewData["ProductImage"] = image;
            ViewData["userAddress"] = address;
            return View();
        }

        public async Task<IActionResult> UpdateOrderProcessing(string OrderStatus, string DeliveryDate, string OrderNumber)
        {
            if ( string.IsNullOrEmpty(OrderStatus) || string.IsNullOrEmpty(OrderNumber) ){ return NotFound(); }
            var order = await _context.Order.Where(x => x.OrderNumber == OrderNumber).FirstOrDefaultAsync();
            order.OrderStatus = OrderStatus;
            DateTime.TryParseExact(DeliveryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);
            order.DeliveryDate = parsedDate;
            if (parsedDate != null)
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                string Subject = "Order Processing";
                string body = "Your order " + order.OrderNumber + " is being processed. Scheduled for delivery on " + parsedDate.ToLongDateString() + ".";
                sendMessage(user, OrderNumber, Subject, body);
                var subject = "Order Processing";
                var message = GenerateEmailForProcessing(user.FirstName + " " + user.LastName, order.OrderNumber, order.OrderDate, order.DeliveryDate);
                await _emailSender.SendEmailAsync(user.Email, subject, message);
            }
            else if  ( OrderStatus != null && OrderStatus == "Out On Delivery")
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                string Subject = "Out On Delivery";
                string body = "Arriving Today. Your order " + order.OrderNumber + " is dispatched for delivery. Contact our support team if you have any concerns.";
                sendMessage(user, OrderNumber, Subject, body);
                var subject = "Out On Delivery";
                var message = GenerateEmailForOutOnDelivery(user.FirstName + " " + user.LastName, order.OrderNumber, order.OrderDate, order.DeliveryDate);
                await _emailSender.SendEmailAsync(user.Email, subject, message);
            }
            else if ( OrderStatus != null && OrderStatus == "Completed")
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                string Subject = "Delivered";
                string body = "Completed. Your order " + order.OrderNumber + " has been successfully delivered.";
                sendMessage(user, OrderNumber, Subject, body);
                var subject = "Delivered";
                var message = GenerateEmailForCompleted(user.FirstName + " " + user.LastName, order.OrderNumber, order.OrderDate, order.DeliveryDate);
                await _emailSender.SendEmailAsync(user.Email, subject, message);
            }
            else if (OrderStatus != null && OrderStatus == "On Hold")
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                string Subject = "On Hold";
                string body = "Your order " + order.OrderNumber + " is currently on hold. If you have any questions, please contact our customer service";
                sendMessage(user, OrderNumber, Subject, body);
                var subject = "On Hold";
                var message = GenerateEmailForOnHold(user.FirstName + " " + user.LastName, order.OrderNumber, order.OrderDate, order.DeliveryDate);
                await _emailSender.SendEmailAsync(user.Email, subject, message);
            }
            else if (OrderStatus != null && OrderStatus == "Cancelled")
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                string Subject = "Cancelled";
                string body = "Your order " + order.OrderNumber + " has been cancelled. If you have any questions, please contact our customer service";
                sendMessage(user, OrderNumber, Subject, body);
                var subject = "Cancelled";
                var message = GenerateEmailForCancelled(user.FirstName + " " + user.LastName, order.OrderNumber, order.OrderDate, order.DeliveryDate);
                await _emailSender.SendEmailAsync(user.Email, subject, message);
            }

            _context.Order.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("OrderPrecessing", "Orders", new { data = OrderNumber });
        }

        public async Task<IActionResult> DeleteOrder(int id, Guid userId)
        {
            if (id != 0 && userId != null)
            {
                var order = await _context.Order.Where(x => x.Id == id && x.UserId == userId.ToString()).Include(x => x.OrderProduct).FirstOrDefaultAsync();
            }else
            {
                return NotFound();
            }
            return RedirectToPage("/Account/Manage/Orders", new { area = "Identity" });
        }
        public static string GenerateEmailForProcessing(string customerName, string orderNumber, DateTime orderDate, DateTime deliveryDate)
        {
            var itemsHtml = string.Empty;
            return $@"
            <div style='font-family: Arial, sans-serif;'>
                <h2 style='color: #2E4053;'>Order Processing</h2>
                <p>Dear {customerName},</p>
                <p>Your order #{orderNumber} placed on {orderDate.ToString("MMMM dd, yyyy")} is being process and Scheduled for delivery on {deliveryDate.ToString("MMMM dd, yyyy")}.</p>
                <p style='margin-top: 20px;'>We hope you enjoy your purchase! If you have any questions, please contact our customer service.</p>
                <p>Best regards,</p>
                <p>Abocar</p>
            </div>";
        }

        public static string GenerateEmailForOutOnDelivery(string customerName, string orderNumber, DateTime orderDate, DateTime deliveryDate)
        {
            var itemsHtml = string.Empty;
            return $@"
            <div style='font-family: Arial, sans-serif;'>
                <h2 style='color: #2E4053;'>Arriving Today</h2>
                <p>Dear {customerName},</p>
                <p>Your order #{orderNumber} placed on {orderDate.ToString("MMMM dd, yyyy")} is dispatched for delivery and will be arriving today.</p>
                <p style='margin-top: 20px;'>We hope you enjoy your purchase! If you have any questions, please contact our customer service.</p>
                <p>Best regards,</p>
                <p>Abocar</p>
            </div>";
        }

        public static string GenerateEmailForCompleted(string customerName, string orderNumber, DateTime orderDate, DateTime deliveryDate)
        {
            var itemsHtml = string.Empty;
            return $@"
            <div style='font-family: Arial, sans-serif;'>
                <h2 style='color: #2E4053;'>Completed</h2>
                <p>Dear {customerName},</p>
                <p>Your order #{orderNumber} placed on {orderDate.ToString("MMMM dd, yyyy")} have been successfully delivered.</p>
                <p style='margin-top: 20px;'>We hope you enjoy your purchase! If you have any questions, please contact our customer service.</p>
                <p>Best regards,</p>
                <p>Abocar</p>
            </div>";
        }

        public static string GenerateEmailForOnHold(string customerName, string orderNumber, DateTime orderDate, DateTime deliveryDate)
        {
            var itemsHtml = string.Empty;
            return $@"
            <div style='font-family: Arial, sans-serif;'>
                <h2 style='color: #2E4053;'>On Hold</h2>
                <p>Dear {customerName},</p>
                <p>Your order #{orderNumber} placed on {orderDate.ToString("MMMM dd, yyyy")} is currently on hold.</p>
                <p style='margin-top: 20px;'>If you have any questions, please contact our customer service.</p>
                <p>Best regards,</p>
                <p>Abocar</p>
            </div>";
        }

        public static string GenerateEmailForCancelled(string customerName, string orderNumber, DateTime orderDate, DateTime deliveryDate)
        {
            var itemsHtml = string.Empty;
            return $@"
            <div style='font-family: Arial, sans-serif;'>
                <h2 style='color: #2E4053;'>Cancelld</h2>
                <p>Dear {customerName},</p>
                <p>Your order #{orderNumber} placed on {orderDate.ToString("MMMM dd, yyyy")} has been cancelled.</p>
                <p style='margin-top: 20px;'>If you have any questions, please contact our customer service.</p>
                <p>Best regards,</p>
                <p>Abocar</p>
            </div>";
        }
    }

}
