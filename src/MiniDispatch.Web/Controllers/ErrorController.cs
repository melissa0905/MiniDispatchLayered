using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniDispatch.Web.Models;

namespace MiniDispatch.Web.Controllers;

public class ErrorController : Controller
{
    [Route("/Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index()
    {
        return View("Error", new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
