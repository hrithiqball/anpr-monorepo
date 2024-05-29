using Microsoft.AspNetCore.Mvc;

namespace MlffWebApi.Controllers;

public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View("~/Views/Dashboard.cshtml");
    }
}