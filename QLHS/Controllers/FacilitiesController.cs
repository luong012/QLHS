using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLHS.Data;
using QLHS.Models;

namespace QLHS.Controllers
{
    public class FacilitiesController : Controller
    {
        private readonly QLHSContext _context;

        public FacilitiesController(QLHSContext context)
        {
            _context = context;
        }

        // GET: Facilities
        public async Task<IActionResult> Index()
        {
            var facility = await _context.Facility.ToListAsync();

            foreach (var item in facility)
            {
                var minPrice = await _context.Room.Where(p => p.Fac_ID == item.Fac_ID)
                    .Join(_context.RoomType,
                    a => a.RoomType_ID,
                    b => b.RoomType_ID,
                    (a, b) => new
                    {
                        price = b.RoomType_Price
                    }).OrderBy(b => b.price).FirstOrDefaultAsync();
                ViewData[$"{item.Fac_ID}"] = minPrice.price.ToString("C0", new CultureInfo("vi-VN"));
            }



            return View(facility);
        }

        public async Task<IActionResult> Find(int? location)
        {
            var facility = await _context.Facility.Where(p => (Math.Truncate((double)p.Fac_ID / 4)) + 1 == location || location == null).ToListAsync();

            int numC = HttpContext.Session.GetInt32("numC") ?? 0;

            DateTime sDate = DateTime.ParseExact(HttpContext.Session.GetString("sDate"), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime eDate = DateTime.ParseExact(HttpContext.Session.GetString("eDate"), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            int sInt = sDate.Year * 10000 + sDate.Month * 100 + sDate.Day;
            int eInt = eDate.Year * 10000 + eDate.Month * 100 + eDate.Day;

            var bookedRoom = await _context.RoomBooking
                .Where(p => p.Booking_Date >= sInt && p.Booking_Date <= eInt)
                .Select(p => p.Room_ID).Distinct()
                .ToListAsync();

            string bookedRoomStr = string.Join(",", bookedRoom);
            HttpContext.Session.SetString("bookedRoom", bookedRoomStr);

            

            foreach (var item in facility)
            {
                var minPrice = await _context.Room.Where(p => p.Fac_ID == item.Fac_ID && !bookedRoom.Contains(p.Room_ID))
                    .Join(_context.RoomType.Where(p => p.RoomType_MaxCusNum >= numC),
                    a => a.RoomType_ID,
                    b => b.RoomType_ID,
                    (a, b) => new
                    {
                        Room = a,
                        RoomType = b,
                    }).OrderBy(b => b.RoomType.RoomType_Price).FirstOrDefaultAsync();

                var availRoom = await _context.Room.Where(p => p.Fac_ID == item.Fac_ID && !bookedRoom.Contains(p.Room_ID))
                    .Join(_context.RoomType.Where(p => p.RoomType_MaxCusNum >= numC),
                    r => r.RoomType_ID,
                    rt => rt.RoomType_ID,
                    (r, rt) => new
                    {
                        Room = r,
                        RoomType = rt,
                    }).CountAsync();



                var nObj = new
                {
                    minPrice = minPrice == null ? "0" : minPrice.RoomType.RoomType_Price.ToString("C0", new CultureInfo("vi-VN")),
                    availRoom = availRoom ,

                };
                ViewData[$"{item.Fac_ID}"] = nObj;
            }


            ViewBag.numC = HttpContext.Session.GetInt32("numC");

            return View(facility);
        }

        // GET: Facilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facility = await _context.Facility
                .FirstOrDefaultAsync(m => m.Fac_ID == id);
            if (facility == null)
            {
                return NotFound();
            }

            return View(facility);
        }

    }
}
