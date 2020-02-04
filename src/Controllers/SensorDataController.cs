using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoomTempDashboard.Models;

namespace RoomTempDashboard.Controllers
{
    public class SensorDataController : Controller
    {
        private readonly HelloIotdbContext _context;

        public SensorDataController(HelloIotdbContext context)
        {
            _context = context;
        }

        // GET: SensorData
        public async Task<IActionResult> Index()
        {
   
            return View(await _context.SensorData.ToListAsync());
        }

        // GET: SensorData/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sensorData = await _context.SensorData
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sensorData == null)
            {
                return NotFound();
            }

            return View(sensorData);
        }

        // GET: SensorData/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SensorData/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Temperature,Humidity,MeasuredAt")] SensorData sensorData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sensorData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sensorData);
        }

        // GET: SensorData/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sensorData = await _context.SensorData.FindAsync(id);
            if (sensorData == null)
            {
                return NotFound();
            }
            return View(sensorData);
        }

        // POST: SensorData/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Temperature,Humidity,MeasuredAt")] SensorData sensorData)
        {
            if (id != sensorData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sensorData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SensorDataExists(sensorData.Id))
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
            return View(sensorData);
        }

        // GET: SensorData/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sensorData = await _context.SensorData
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sensorData == null)
            {
                return NotFound();
            }

            return View(sensorData);
        }

        // POST: SensorData/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sensorData = await _context.SensorData.FindAsync(id);
            _context.SensorData.Remove(sensorData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SensorDataExists(int id)
        {
            return _context.SensorData.Any(e => e.Id == id);
        }
    }
}
