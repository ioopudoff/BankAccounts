using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using BankAccounts.Models;

namespace BankAccounts.Controllers
{
    public class HomeController : Controller
    {
        private BankContext dbContext;
        public HomeController(BankContext context)
        {
            dbContext = context;
        }
        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Данный Email уже занят.");
                    return View("index", newUser);
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

                dbContext.Add(newUser);
                dbContext.SaveChanges();

                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                int id = newUser.UserId;

                return Redirect($"/account/{newUser.UserId}");

            }
            return View("index");
        }

        [HttpGet("account/{UserId}")]
        public IActionResult Account(User user)
        {
            int? LoginCheck = HttpContext.Session.GetInt32("UserId");
            if (LoginCheck == null)
            {
                return Redirect("/login");
            }
            ViewBag.Total = dbContext.Transactions.Where(u => u.UserId == user.UserId).Sum(x => x.Amount);
            ViewBag.user = dbContext.Users.FirstOrDefault(u => u.UserId == LoginCheck);
            ViewBag.trans = dbContext.Transactions.Where(u => u.UserId == user.UserId).OrderByDescending(l => l.CreatedAt).ToList();
            return View("account");
        }

        [HttpGet("login")]
        public IActionResult LoginScreen()
        {
            return View("login");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);
                if (userInDb == null)
                {
                    ModelState.AddModelError("Email", "Некорретный Email или Пароль");
                    return View("login");
                }
                else
                {
                    var hasher = new PasswordHasher<LoginUser>();
                    var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);
                    if (result == 0)
                    {
                        ModelState.AddModelError("Email", "Некорретный Email или Пароль");
                        return View("login");
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                        return Redirect($"/account/{userInDb.UserId}");
                    }
                }
            }
            return View("login");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("login");
        }

        [HttpPost("/createtransaction")]
        public IActionResult CreateTransaction(Transaction trans)
        {
            int? sesh = HttpContext.Session.GetInt32("UserId");
            User user = dbContext.Users.FirstOrDefault(u => u.UserId == sesh);
            decimal balance = dbContext.Transactions.Where(u => u.UserId == user.UserId).Sum(x => x.Amount);
            decimal amount = trans.Amount;
            if (amount < 0)
            {
                if (Math.Abs(amount) > balance)
                {
                    ModelState.AddModelError("Amount", "Вы не можете снять больше, чем есть на счету");
                    return Redirect($"/account/{user.UserId}");
                }
            }
            dbContext.Transactions.Add(trans);
            dbContext.SaveChanges();
            return Redirect($"/account/{user.UserId}");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
