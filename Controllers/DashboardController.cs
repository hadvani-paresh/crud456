using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CrudProject.Controllers
{
    // [Authorize] // Uncomment this if you have configured Authentication properly in Program.cs
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
