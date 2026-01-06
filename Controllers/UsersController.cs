using Microsoft.AspNetCore.Mvc;
using CrudProject.Models;
using System.Text.Json;

namespace CrudProject.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
