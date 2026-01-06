using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CrudProject.Models;

namespace CrudProject.Controllers;

[Microsoft.AspNetCore.Authorization.Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Home/Index accessed by user: {User}", User.Identity?.Name ?? "Anonymous");
        _logger.LogInformation("User authenticated: {IsAuthenticated}", User.Identity?.IsAuthenticated ?? false);
        _logger.LogInformation("Authentication type: {AuthType}", User.Identity?.AuthenticationType ?? "None");
        _logger.LogInformation("Claims count: {ClaimsCount}", User.Claims.Count());
        
        foreach (var claim in User.Claims)
        {
            _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
        }
        
        return View();
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
