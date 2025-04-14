using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Diagnostics;
using TURF_BOOKINGS.Filters;
using TURF_BOOKINGS.Models;
using TURF_BOOKINGS.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;


namespace TURF_BOOKINGS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult LogViews()
        {

            return View();  
        }

        public IActionResult Homesss()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.Email = userEmail;
            ViewBag.UserName = userName;
            return View();
        }


        public IActionResult AboutUS()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.Email = userEmail;
            ViewBag.UserName = userName;
            return View();
        }

        [SessionAuthorize]
        public IActionResult Booking()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.Email = userEmail;
            ViewBag.UserName = userName;
            return View();
        }

        
        public IActionResult Services()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.Email = userEmail;
            return View();
        }


        public IActionResult Coaches()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.Email = userEmail;
            ViewBag.UserName = userName;
            return View();
        }

        public IActionResult RegView()
        {
            return View();
        }

        public IActionResult ContactUS()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.Email = userEmail;
            ViewBag.UserName = userName;
            return View();
        }

        [HttpGet]
        [SessionAuthorize]
        public IActionResult Update()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var currentDateTime = DateTime.Now;

            ViewBag.History = _context.Booking
                .Where(x => x.Email == userEmail && x.Date <= currentDateTime && x.isDeleted != 1)
                .ToList();

            ViewBag.Pending = _context.Booking
                .Where(x => x.Email == userEmail && x.Date > currentDateTime && x.isDeleted != 1)
                .ToList();

            ViewBag.Email = userEmail;

            return View();
        }

        [HttpGet]
        public IActionResult VerifyOtp()
        {
            var name = HttpContext.Session.GetString("TempUserName");
            var email = HttpContext.Session.GetString("TempUserEmail");
            var generatedOtp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("GeneratedOtp", generatedOtp);
            ViewBag.Otp = generatedOtp;
            ViewBag.Name = name.ToString();
            ViewBag.Email = email.ToString();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string enteredOtp)
        {
            var storedOtp = HttpContext.Session.GetString("GeneratedOtp");

            if (enteredOtp == storedOtp)
            {
                
                var name = HttpContext.Session.GetString("TempUserName");
                var email = HttpContext.Session.GetString("TempUserEmail");
                var phone = HttpContext.Session.GetString("TempUserPhone");
                var password = HttpContext.Session.GetString("TempUserPassword");

                var salt = BCrypt.Net.BCrypt.GenerateSalt();
                var HashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);    

                var newUser = new RegisterModel
                {
                    Name = name,
                    Email = email,
                    Phone = phone,
                    Password = HashedPassword
                };

                _context.User.Add(newUser);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("UserName", newUser.Name);
                HttpContext.Session.SetString("UserEmail", newUser.Email);

                return RedirectToAction("Homesss", "Home");
            }
            else
            {
                ViewBag.Error = "Invalid OTP. Please try again.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegView(RegisterModel register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }

            var existingUser = await _context.User
                .FirstOrDefaultAsync(reg => reg.Email == register.Email);

            if (existingUser != null)
            {
                return RedirectToAction("LogViews", "Home");
            }
            else
            {
                
                HttpContext.Session.SetString("TempUserName", register.Name);
                HttpContext.Session.SetString("TempUserEmail", register.Email);
                HttpContext.Session.SetString("TempUserPhone", register.Phone);
                HttpContext.Session.SetString("TempUserPassword", register.Password);

                var userEmail = HttpContext.Session.GetString("TempUserEmail");
                ViewBag.Email = userEmail;

                var userName = HttpContext.Session.GetString("TempUserName");
                ViewBag.UserName = userName;

                //ViewBag.Otp = generatedOtp;
                //ViewBag.Name = register.Name;
                //ViewBag.Email = register.Email;

                return RedirectToAction("VerifyOtp");
            }
        }


        //[HttpPost]
        //public async Task<IActionResult> RegView(RegisterModel register)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    var existingUser = _context.Register.FirstOrDefault(reg => reg.Email == register.Email);
        //    if (existingUser != null)
        //    {
        //        ModelState.AddModelError("Email", "A user with this email already exists.");
        //        return RedirectToAction("LogView");
        //    }
        //    else
        //    {
        //        _context.Register.Add(register);
        //        await _context.SaveChangesAsync();

        //        RegisterModel newlyRegisteredUser = await _context.Register.FirstOrDefaultAsync(u => u.Email == register.Email && u.Password == register.Password);
        //        if (newlyRegisteredUser != null)
        //        {
        //            HttpContext.Session.SetString("UserName", newlyRegisteredUser.Name);
        //            HttpContext.Session.SetString("UserEmail", newlyRegisteredUser.Email);
        //        }
        //        return RedirectToAction("Homesss");
        //    }
        //}




        [HttpPost]
        public async Task<IActionResult> LogViews(LogViewDTO cred)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                
                var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Email == cred.Email);
                if (existingUser == null)
                {
                    TempData["warning"] = "Invalid email or password.";
                    return View();
                }
                if(existingUser != null)
                {
                    bool isValid = BCrypt.Net.BCrypt.Verify(cred.Password , existingUser.Password);
                    if (isValid)
                    {
                        HttpContext.Session.SetString("UserName", existingUser.Name);
                        HttpContext.Session.SetString("UserEmail", existingUser.Email);


                        var userEmail = HttpContext.Session.GetString("UserEmail");
                        var userName = HttpContext.Session.GetString("UserName");
                        ViewBag.Email = userEmail;
                        ViewBag.UserName = userName;

                        return RedirectToAction("Homesss");
                    }
                    else
                    {
                        return View();
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in.");
                TempData["warning"] = "An error occurred while processing your request.";
                return RedirectToAction("LogView");
            }
            return View();

        }



        //[HttpPost]
        //[SessionAuthorize]
        //public async Task<IActionResult> BookingView()
        //{
        //    var adminEmail = HttpContext.Session.GetString("UserEmail");
        //    if (adminEmail == "admin123@gmail.com")
        //    {
        //        var bookings = await _context.Booking.ToListAsync();
        //        return View(bookings);
        //    }
        //    else
        //    {
        //        return RedirectToAction("LogView");
        //    }
        //}




        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(DateTime date)
        {
            try
            {
                _logger.LogInformation("Fetching booked slots for date: {Date}", date);

                
                var bookings = await _context.Booking
                    .Where(b => b.Date == date && b.isDeleted != 1)
                    .ToListAsync();

               
                var bookedSlots = bookings.SelectMany(b => b.TimeSlots).ToList();

                _logger.LogInformation("Booked slots retrieved: {BookedSlots}", string.Join(", ", bookedSlots));

                var allSlots = new List<string>
            {
                "00:00 - 01:00", "01:00 - 02:00", "02:00 - 03:00", "03:00 - 04:00",
                "04:00 - 05:00", "05:00 - 06:00", "06:00 - 07:00", "07:00 - 08:00",
                "08:00 - 09:00", "09:00 - 10:00", "10:00 - 11:00", "11:00 - 12:00",
                "12:00 - 13:00", "13:00 - 14:00", "14:00 - 15:00", "15:00 - 16:00",
                "16:00 - 17:00", "17:00 - 18:00", "18:00 - 19:00", "19:00 - 20:00",
                "20:00 - 21:00", "21:00 - 22:00", "22:00 - 23:00", "23:00 - 00:00"
            };

                var availableSlots = allSlots.Except(bookedSlots).ToList();
                _logger.LogInformation("Available slots calculated: {AvailableSlots}", string.Join(", ", availableSlots));

                return PartialView("_AvailableSlots", availableSlots);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching available slots.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [SessionAuthorize]
        public async Task<IActionResult> Booking(bookingDTO booking)
        {
            if (!ModelState.IsValid)
            {
                return View(); 
            }

            if (booking.Date < DateTime.Today)
            {
                ModelState.AddModelError("Date", "Cannot book a slot for a previous date.");
                return View();
            }

            try
            {
                var userName = HttpContext.Session.GetString("UserName");
                var userEmail = HttpContext.Session.GetString("UserEmail");

                var existingBookings = await _context.Booking
                    .Where(b => b.Date == booking.Date)
                    .ToListAsync();

                var bookedSlots = existingBookings.SelectMany(b => b.TimeSlots).ToList();

                if (booking.TimeSlots.Any(slot => bookedSlots.Contains(slot)))
                {
                    ModelState.AddModelError("", "One or more selected time slots are already booked.");
                    return View();
                }

                
                var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Email == userEmail && u.Name == userName);
                if (existingUser != null)
                {
                    
                    BookingModel newBooking = new BookingModel
                    {
                        Email = userEmail,
                        Name = userName,
                        TimeSlots = booking.TimeSlots,
                        Date = booking.Date,
                        Amount = booking.TimeSlots.Count * 800
                    };

                   
                    _context.Booking.Add(newBooking);
                    await _context.SaveChangesAsync(); 

                    TempData["success"] = "Your attempt to book a slot was successful!";
                    return RedirectToAction("Homesss");
                }
                else
                {
                    return View("RegView");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the booking.");
                TempData["warning"] = "An error occurred while processing your request.";
                return RedirectToAction("Error");
            }
        }



        [HttpPost]
        [SessionAuthorize]
        public async Task<IActionResult> Update(UpdateDTO booking)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid.");
                return View(booking); 
            }

            try
            {
                var existingSlot = await _context.Booking.FirstOrDefaultAsync(x => x.Id == booking.Id);

                if (existingSlot == null)
                {
                    _logger.LogWarning("Booking not found with ID: {BookingId}", booking.Id);
                    ModelState.AddModelError("", "Booking not found.");
                    return View(booking); 
                }

                var existingBookings = await _context.Booking
                    .Where(b => b.Date == existingSlot.Date && b.Id != existingSlot.Id)
                    .ToListAsync();

                
                var bookedSlots = existingBookings.SelectMany(b => b.TimeSlots).ToList();

                if (booking.TimeSlots.Any(slot => bookedSlots.Contains(slot)))
                {
                    _logger.LogWarning("Conflicting bookings found for the selected time slots on date: {Date}", existingSlot.Date);
                    ModelState.AddModelError("", "One or more selected time slots are already booked.");
                    return View(booking); 
                }

                existingSlot.TimeSlots = booking.TimeSlots;
                existingSlot.Amount = booking.TimeSlots.Count * 800;

                _context.Booking.Update(existingSlot);
                await _context.SaveChangesAsync();

                TempData["success"] = "Your attempt to update the booking was successful!";
                return RedirectToAction("Update");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the update.");
                TempData["warning"] = "An error occurred while processing your request.";
                return RedirectToAction("Error");
            }
        }




        [HttpPost]
        [SessionAuthorize]
        public IActionResult Delete(UpdateDTO cred)
        {
            try
            {
                var existingSlot = _context.Booking.FirstOrDefault(x => x.Id == cred.Id);
                if (existingSlot != null)
                {
                    existingSlot.isDeleted = 1;
                    _context.Booking.Update(existingSlot);
                    _context.SaveChanges();
                    return RedirectToAction("Update");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the booking.");
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Homesss");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }





    }
}
