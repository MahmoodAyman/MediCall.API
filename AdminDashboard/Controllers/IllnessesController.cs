using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Core.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Core.Interface;

namespace AdminDashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class IllnessesController : Controller
    {
        private readonly IGenericRepository<Illness> _illnessRepository;

        public IllnessesController(IGenericRepository<Illness> illnessRepository)
        {
            _illnessRepository = illnessRepository;
        }

        // GET: Illnesses
        public async Task<IActionResult> Index()
        {
            return View(await _illnessRepository.GetAllAsync());
        }

        // GET: Illnesses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var illness = await _illnessRepository.GetByIdAsync(id);
            if (illness == null)
            {
                return NotFound();
            }

            return View(illness);
        }

        // GET: Illnesses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Illnesses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,IsDeleted,CreatedAt,UpdatedAt")] Illness illness)
        {
            if (ModelState.IsValid)
            {
                await _illnessRepository.AddAsync(illness);
                await _illnessRepository.SaveAllAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(illness);
        }

        // GET: Illnesses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var illness = await _illnessRepository.GetByIdAsync(id);
            if (illness == null)
            {
                return NotFound();
            }
            return View(illness);
        }

        // POST: Illnesses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,IsDeleted,CreatedAt,UpdatedAt")] Illness illness)
        {
            if (id != illness.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _illnessRepository.Update(illness);
                    await _illnessRepository.SaveAllAsync();;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IllnessExists(illness.Id))
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
            return View(illness);
        }

        // GET: Illnesses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var illness = await _illnessRepository.GetByIdAsync(id);
            if (illness == null)
            {
                return NotFound();
            }

            return View(illness);
        }

        // POST: Illnesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var illness = await _illnessRepository.GetByIdAsync(id);
            if (illness != null)
            {
                await _illnessRepository.DeleteByIdAsync(id);
            }

            await _illnessRepository.SaveAllAsync();;
            return RedirectToAction(nameof(Index));
        }

        private bool IllnessExists(int id)
        {
            return _illnessRepository.GetByIdAsync(id).Result is not null;
        }
    }
}
