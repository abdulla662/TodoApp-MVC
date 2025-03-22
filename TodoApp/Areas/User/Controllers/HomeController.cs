using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TodoApp.DataAccess;
using TodoApp.Models;

namespace TodoApp.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

  

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult error()
        {
            return View();
        }



    }
}
