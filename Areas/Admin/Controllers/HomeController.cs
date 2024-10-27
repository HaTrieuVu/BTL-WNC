using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BTL.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles = "Admin")]
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
