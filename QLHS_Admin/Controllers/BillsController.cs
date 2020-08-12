using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLHS_Admin.Data;
using QLHS_Admin.Models;

namespace QLHS_Admin.Controllers
{
    public class BillsController : Controller
    {
        private readonly QLHSContext _context;

        public BillsController(QLHSContext context)
        {
            _context = context;
        }

        // GET: Bills
        public async Task<IActionResult> Index()
        {
            return View(await _context.Bill.OrderByDescending(p => p.Bill_ID).ToListAsync());
        }

        // GET: Bills/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill
                .FirstOrDefaultAsync(m => m.Bill_ID == id);

            var room = await _context.RoomBooking.Where(p => p.Bill_ID == id)
                .GroupBy(d => new { d.Room_ID, d.Bill_ID })
                .Select(m => new { m.Key.Room_ID, m.Key.Bill_ID })
                .Join(_context.Room,
                      rb => rb.Room_ID,
                      r => r.Room_ID,
                      (rb, r) => new
                      {
                          Room = r
                      }).ToListAsync();

            var roomlist = new List<RoomVM>();

            foreach (var item in room)
            {
                var facility = await _context.Facility.Where(p => p.Fac_ID == item.Room.Fac_ID).FirstOrDefaultAsync();
                var roomtype = await _context.RoomType.Where(p => p.RoomType_ID == item.Room.RoomType_ID).FirstOrDefaultAsync();
                var fL = new List<Facility>();
                var rL = new List<RoomType>();
                fL.Add(facility);
                rL.Add(roomtype);

                var roomvmitem = new RoomVM
                {
                    Room = item.Room,
                    RoomTypes = rL,
                    Facilities = fL,
                };
                roomlist.Add(roomvmitem);
            }

            var service = await _context.Service_Detail
                .Join(_context.Service,
                sd => sd.Service_ID,
                s => s.Service_ID,
                (sd, s) => new
                {
                    Service = s
                }).ToListAsync();
            var servicelist = new List<ServiceVM>();

            foreach (var item in service)
            {
                var ser_qty = await _context.Service_Detail.Where(p => p.Bill_ID == id && p.Service_ID == item.Service.Service_ID).FirstOrDefaultAsync();
                var ser_item = new ServiceVM
                {
                    Service = item.Service,
                    Service_Quantity = ser_qty.Service_Quantity,
                };
                servicelist.Add(ser_item);
            }
            BillVM billVM = new BillVM
            {
                Bill = bill,
                RoomVMs = roomlist,
                ServiceVMs = servicelist,
            };



            if (bill == null)
            {
                return NotFound();
            }

            return View(billVM);
        }

        public IActionResult AddService(int? id)
        {
            var bill = _context.Bill
                .FirstOrDefault(m => m.Bill_ID == id);

            var service = _context.Service_Detail.Where(p => p.Bill_ID == id)
               .Join(_context.Service,
               sd => sd.Service_ID,
               s => s.Service_ID,
               (sd, s) => new
               {
                   Service = s
               }).ToList();

            var servicelist = new List<ServiceVM>();

            foreach (var item in service)
            {
                var ser_qty = _context.Service_Detail.Where(p => p.Bill_ID == id && p.Service_ID == item.Service.Service_ID).FirstOrDefault();
                var ser_item = new ServiceVM
                {
                    Service = item.Service,
                    Service_Quantity = ser_qty.Service_Quantity,
                };
                servicelist.Add(ser_item);
            }
            BillVM billVM = new BillVM
            {
                Bill = bill,
                ServiceVMs = servicelist,
            };
            return View();
        }

        public async Task<IActionResult> Forward(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill.FindAsync(id);
            bill.Bill_Status = 2;
            _context.Update(bill);
            _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        // GET: Bills/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bills/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Bill_ID,Emp_ID,Cus_ID,Bill_Status,Bill_CheckInDate,Bill_CheckOutDate,Bill_CusNum,Bill_BookCost,Bill_ExtraCost,Bill_Total")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bill);
        }

        // GET: Bills/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }
            return View(bill);
        }

        // POST: Bills/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Bill_ID,Emp_ID,Cus_ID,Bill_Status,Bill_CheckInDate,Bill_CheckOutDate,Bill_CusNum,Bill_BookCost,Bill_ExtraCost,Bill_Total")] Bill bill)
        {
            if (id != bill.Bill_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillExists(bill.Bill_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bill);
        }

        // GET: Bills/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill
                .FirstOrDefaultAsync(m => m.Bill_ID == id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bill = await _context.Bill.FindAsync(id);
            _context.Bill.Remove(bill);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillExists(int id)
        {
            return _context.Bill.Any(e => e.Bill_ID == id);
        }
    }
}
