using Microsoft.AspNetCore.Mvc;

namespace TodoApp.Areas.ErrorPage
{
    [Area("ErrorPage")]
    public class AccessDeniedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public  IActionResult Errormodel()
        {
            return View();
        }
    }
}
