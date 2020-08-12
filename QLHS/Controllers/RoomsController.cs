using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QLHS.Data;
using QLHS.Models;

namespace QLHS.Controllers
{
    public class RoomsController : Controller
    {
        private readonly QLHSContext _context;

        public RoomsController(QLHSContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            //var room = await _context.Room.ToListAsync();
            //var roomtype = await _context.RoomType.Where(rt=>rt.RoomType_ID==room.r);

            ////var feature = await _context.RoomType_Feature
            ////    .Join(_context.Feature,
            ////            rt => rt.Feature_ID,
            ////            ft => ft.Feature_ID,
            ////            (rt, ft) => new
            ////            {
            ////                RoomType_Feature = rt,
            ////                Feature = ft
            ////            }).Where(p=> p.RoomType_Feature.RoomType_ID == room.RoomType_ID).ToListAsync();

            var rwithrt = await _context.Room
                .Join(_context.RoomType,
                        r => r.RoomType_ID,
                        rt => rt.RoomType_ID,
                        (r, rt) => Tuple.Create(r,rt)).ToListAsync();
            var tupleListSorted = from thing in rwithrt
                                  orderby thing.Item2.RoomType_Price
                                  select thing;
            return View(tupleListSorted);
        }
        public async Task<IActionResult> Find(int? facID, int? numC)
        {
            var rwithrt = await _context.Room.Where(p => p.Fac_ID == facID)
                .Join(_context.RoomType,
                        r => r.RoomType_ID,
                        rt => rt.RoomType_ID,
                        (r, rt) => Tuple.Create(r, rt)).ToListAsync();

            var newT = new List<Tuple<Room, RoomType, int>>();

            

            List<int> bookedRoom = new List<int>();
            string bookedRoomStr = HttpContext.Session.GetString("bookedRoom") ?? "";
            if (bookedRoomStr != "")
            {
                List<string> bookedRoomL = bookedRoomStr.Split(',').ToList();
                bookedRoom = bookedRoomL.Select(int.Parse).ToList();
            }



            foreach (var item in rwithrt)
            {
                int i = 0;
                if (item.Item2.RoomType_MaxCusNum < HttpContext.Session.GetInt32("numC")) i = 1;
                if (bookedRoom.Contains(item.Item1.Room_ID)) i = 2;
                newT.Add(new Tuple<Room, RoomType, int>(item.Item1, item.Item2, i));
            }

            var tupleListSorted = from thing in newT
                                  orderby thing.Item3, thing.Item2.RoomType_Price
                                  select thing;



            ViewBag.FacName = await _context.Facility.Where(p => p.Fac_ID == facID).Select(p => p.Fac_Name).FirstOrDefaultAsync();

            

            return View(tupleListSorted);
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int newid = id ?? 0;

            var room = await _context.Room
                .FirstOrDefaultAsync(m => m.Room_ID == id);
            if (room == null)
            {
                return NotFound();
            }

            var roomtype = await _context.RoomType.FirstOrDefaultAsync(m => m.RoomType_ID == room.RoomType_ID);
            if (roomtype == null)
            {
                return NotFound();
            }
            var fac = await _context.Facility.FirstOrDefaultAsync(m => m.Fac_ID == room.Fac_ID);
            if (fac == null)
            {
                return NotFound();
            }

            //var feature = await _context.RoomType_Feature
            //    .Join(_context.Feature,
            //            rt => rt.Feature_ID,
            //            ft => ft.Feature_ID,
            //            (rt, ft) => new
            //            {
            //                RoomType_Feature = rt,
            //                Feature = ft
            //            }).Where(p=> p.RoomType_Feature.RoomType_ID == room.RoomType_ID).ToListAsync();

            //ViewBag.ListFT = feature;


            var feature = await _context.RoomType_Feature.Where(rf=>rf.RoomType.RoomType_ID==room.RoomType_ID).Select(rf=>rf.Feature).ToListAsync();
            ViewBag.ListFT = feature;


            var obj = new
            {
                ID = room.Room_ID,
                Img = room.Room_Img,
                Name = roomtype.RoomType_Name,
                Desc = roomtype.RoomType_Desc,
                Price = roomtype.RoomType_Price,
                MaxCusNum = roomtype.RoomType_MaxCusNum,
                Size = roomtype.RoomType_Size,
                FacID = room.Fac_ID,
                Fac = fac.Fac_Name,
                Address = fac.Fac_Address,
                Lat = fac.Fac_Latitude,
                Long = fac.Fac_Longitude,
            };

            ViewData["numC"] = HttpContext.Session.GetInt32("numC") ?? 0;
            ViewData["Flag"] = 0;
            string bookedRoomStr = HttpContext.Session.GetString("bookedRoom") ?? "";
            if (bookedRoomStr != "")
            {
                List<string> bookedRoom = bookedRoomStr.Split(',').ToList();
                List<int> bookedRoomID = bookedRoom.Select(int.Parse).ToList();
                if (bookedRoomID.Contains(newid)) ViewData["Flag"] = 1;
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(int roomID, string roomType, string facility, double price)
        {
            var obj = new
            {
                Room = roomID,
                RoomType = roomType,
                Facility = facility,
                Price = price,
            };
            var list = new Dictionary<int, Object>();
            string cartStr = HttpContext.Session.GetString("cart")??"";
            if (cartStr == "")
            {
                list.Add(roomID, obj);
                
                HttpContext.Session.SetString("cart",JsonConvert.SerializeObject(list));

                HttpContext.Session.SetInt32("total", 1);
            }
            else
            {
                list = JsonConvert.DeserializeObject<Dictionary<int, Object>>(cartStr);
                if (!list.ContainsKey(roomID))
                {
                    list.Add(roomID, obj);
                    HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(list));
                    int qty = HttpContext.Session.GetInt32("total") ?? 0;
                    qty += 1;
                    HttpContext.Session.SetInt32("total", qty);
                }
                
            }
            return RedirectToAction("Details", new { id = roomID });
        }
       
    }
}
