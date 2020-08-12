using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace QLHS.Controllers
{
    [Route("Error")]
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("404")]
        public IActionResult PageNotFound()
        {
            string originalPath = "unknown";
            if (HttpContext.Items.ContainsKey("originalPath"))
            {
                originalPath = HttpContext.Items["originalPath"] as string;
            }
            return View();
        }
    }
}