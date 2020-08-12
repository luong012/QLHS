using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QLHS.Models;

namespace QLHS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.sDate = HttpContext.Session.GetString("sDate") ?? null;
            ViewBag.eDate = HttpContext.Session.GetString("eDate") ?? null;
            ViewBag.numC = HttpContext.Session.GetInt32("numC") ?? null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(int? location, int? numC, string? sDate, string? eDate)
        {
            //string sString = sDate.ToString().Substring(6, 2) + sDate.ToString().Substring(0, 2) + sDate.ToString().Substring(3, 2);
            //string eString = eDate.ToString().Substring(6, 2) + eDate.ToString().Substring(0, 2) + eDate.ToString().Substring(3, 2);

           
            HttpContext.Session.SetInt32("numC", numC ?? 0);
            HttpContext.Session.SetString("sDate", sDate ?? "");
            HttpContext.Session.SetString("eDate", eDate ?? "");
            HttpContext.Session.SetString("cart", "");
            HttpContext.Session.SetInt32("total",0);



            return RedirectToAction("Find", "Facilities", new { location = location});
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
}
