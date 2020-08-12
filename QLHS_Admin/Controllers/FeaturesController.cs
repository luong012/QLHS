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
    public class FeaturesController : Controller
    {
        private readonly QLHSContext _context;

        public FeaturesController(QLHSContext context)
        {
            _context = context;
        }

        // GET: Features
        public async Task<IActionResult> Index()
        {
            return View(await _context.Feature.ToListAsync());
        }

        // GET: Features/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feature = await _context.Feature
                .FirstOrDefaultAsync(m => m.Feature_ID == id);
            if (feature == null)
            {
                return NotFound();
            }

            return View(feature);
        }

        // GET: Features/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Features/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Feature_ID,Feature_Name,Feature_Icon")] Feature feature)
        {
            if (ModelState.IsValid)
            {
                _context.Add(feature);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feature);
        }

        // GET: Features/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feature = await _context.Feature.FindAsync(id);
            if (feature == null)
            {
                return NotFound();
            }
            return View(feature);
        }

        // POST: Features/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Feature_ID,Feature_Name,Feature_Icon")] Feature feature)
        {
            if (id != feature.Feature_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feature);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeatureExists(feature.Feature_ID))
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
            return View(feature);
        }

        // GET: Features/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feature = await _context.Feature
                .FirstOrDefaultAsync(m => m.Feature_ID == id);
            if (feature == null)
            {
                return NotFound();
            }

            return View(feature);
        }

        // POST: Features/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feature = await _context.Feature.FindAsync(id);
            _context.Feature.Remove(feature);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeatureExists(int id)
        {
            return _context.Feature.Any(e => e.Feature_ID == id);
        }
    }
}
