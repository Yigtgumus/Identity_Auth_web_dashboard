using Microsoft.AspNetCore.Mvc;

namespace Identity_Auth.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Database()
        {
            return RedirectToAction("Connect", "UserDatabase");
        }
        public IActionResult Reports()
        {
            return View();
        }
       
    }
}
