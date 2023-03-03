using H5ServerSideProgrammering.Data;
using H5ServerSideProgrammering.Models;
using H5ServerSideProgrammering.Models.DB;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BC = BCrypt.Net.BCrypt;

namespace H5ServerSideProgrammering.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly H5DbContext _context;
        private readonly IDataProtector _dataProtector;

        public HomeController(ILogger<HomeController> logger, H5DbContext context, IDataProtectionProvider dataProtectionProvider)
        {
            _logger = logger;
            _context = context;
            _dataProtector = dataProtectionProvider.CreateProtector("A2FEFA94-1DD0-426F-8754-75F76876D164");
        }
        [HttpGet]
        public IActionResult Index()
        {
            var loginId = HttpContext.Session.GetInt32("loginId");
            ViewBag.loginId = loginId;
            return View();
        }
        [HttpPost]
        public IActionResult Index(string username, string password)
        { 
            var login = _context.Login.SingleOrDefault(l => l.Username == username);
            if (login != null)
            {
                try
                {
                    if (BC.Verify(password, login.Password))
                    {
                        HttpContext.Session.SetInt32("loginId", login.Id);
                        return Redirect("/home/RegisterTodo");
                    }
                    else
                    {
                        ViewBag.Username = username;
                        ViewBag.Message = "Forkert Adgangskode";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ViewBag.Message = ex.Message;
                }
            }
            else
            {
                ViewBag.Username = username;
                ViewBag.Message = "Forkert Brugernavn";
            }
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            var loginId = HttpContext.Session.GetInt32("loginId");
            ViewBag.loginId = loginId;
            ViewBag.Login = new Login { Username = "", Email = "" , Password = "" };
            return View();
        }
        [HttpPost]
        public IActionResult Register(string username, string password, string repeatPass)
        {
            if (password != repeatPass)
            {
                ViewBag.Message = "Adgangskode matcher ikke";
                ViewBag.Login = new Login { Username = "", Email = "", Password = "" };
                return View();
            }
            Login login = new Login()
            {
                Password = BC.HashPassword(password),
                Username = username
            };
            _context.Login.Add(login);
            _context.SaveChanges();
            return Redirect("/");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }
        [HttpGet]
        public IActionResult RegisterTodo()
        {
            var loginId = HttpContext.Session.GetInt32("loginId");
            ViewBag.loginId = loginId;
            if (loginId == null)
            {
                return Redirect("/");
            }
            List<TodoItem> todos = _context.TodoItem.Where(x => x.LoginId == (int)HttpContext.Session.GetInt32("loginId")).ToList();
            foreach (TodoItem item in todos)
            {
                try
                {
                    item.Title = _dataProtector.Unprotect(item.Title);
                    item.Description = _dataProtector.Unprotect(item.Description);
                    item.Added = item.Added;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            ViewBag.todos = todos;
            ViewBag.todo = new TodoItem { 
                Title = "",
                Description = ""
            };
            return View();
        }
        [HttpPost]
        public IActionResult RegisterTodo(string title, string description)
        {
            TodoItem item = new TodoItem()
            {
                Title = _dataProtector.Protect(title),
                Description = _dataProtector.Protect(description),
                Added = DateTime.Now,
                LoginId = (int) HttpContext.Session.GetInt32("loginId")
            };
            _context.TodoItem.Add(item);
            _context.SaveChanges();
            return Redirect("/home/RegisterTodo");
        }
        [HttpGet("home/RegisterTodo/Edit/{id}")]
        public IActionResult RegisterTodo(int id, string i)
        {
            var loginId = HttpContext.Session.GetInt32("loginId");
            ViewBag.loginId = loginId;
            if (loginId == null)
            {
                return Redirect("/");
            }
            List<TodoItem> todos = _context.TodoItem.Where(x => x.LoginId == (int)loginId).ToList();
            foreach (TodoItem item in todos)
            {
                try
                {
                    item.Title = _dataProtector.Unprotect(item.Title);
                    item.Description = _dataProtector.Unprotect(item.Description);
                    item.Added = item.Added;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            ViewBag.todos = todos;

            TodoItem iten = _context.TodoItem.FirstOrDefault(x => x.Id == id && x.LoginId == (int)loginId);
            if (iten != null)
            {
                ViewBag.todo = iten;
            }
            else
            {
                return Redirect("/home/RegisterTodo");
            }
            return View();
        }
        [HttpPost("home/RegisterTodo/Edit/{id}")]
        public IActionResult RegisterTodo(int id, string title, string description)
        {
            TodoItem iten = _context.TodoItem.FirstOrDefault(x => x.Id == id);
            if (iten != null)
            {
                iten.Title = _dataProtector.Protect(title);
                iten.Description = _dataProtector.Protect(description);
                _context.SaveChanges();
            }
            return Redirect("/home/RegisterTodo");
        }
        [HttpGet("home/RegisterTodo/Delete/{id}")]
        public IActionResult RegisterTodo(int id)
        {
            var loginId = HttpContext.Session.GetInt32("loginId");
            ViewBag.loginId = loginId;
            if (loginId == null)
            {
                return Redirect("/");
            }
            TodoItem iten = _context.TodoItem.FirstOrDefault(x => x.Id == id && x.LoginId == (int)loginId);
            if (iten != null)
            {
                _context.TodoItem.Remove(iten);
                _context.SaveChanges();
            }
            return Redirect("/home/RegisterTodo");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}