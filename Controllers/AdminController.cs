using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using TURF_BOOKINGS.Services;
using TURF_BOOKINGS.Models;

namespace TURF_BOOKINGS.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;

        public AdminController(ILogger<AdminController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult BookingView()
        {
            ViewBag.Email = "srfitnessvillage@gmail.com";
            try
            {
                var currentDateTime = DateTime.Now;

                var historyBookings = _context.Booking
                    .Where(x => x.Date <= currentDateTime && x.isDeleted != 1)
                    .ToList();

                var pendingBookings = _context.Booking
                    .Where(x => x.Date > currentDateTime && x.isDeleted != 1)
                    .ToList();

                var totalAmountPaid = historyBookings.Sum(x => x.Amount);
                var totalAmountRemaining = pendingBookings.Sum(x => x.Amount);

                ViewBag.TotalAmountPaid = totalAmountPaid;
                ViewBag.TotalAmountRemaining = totalAmountRemaining;

                ViewBag.History = historyBookings;
                ViewBag.Pending = pendingBookings;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking data.");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Users()
        {
            ViewBag.Email = "srfitnessvillage@gmail.com";
            var users = _context.User.Where(x => x.Email != null).ToList(); 
            ViewBag.Users = users;
            return View();
        }
    }
}
