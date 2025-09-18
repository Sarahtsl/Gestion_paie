using Gestion_paie.DataBase;
using Gestion_paie.Models;
using GestionPaie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;


public class BenefitTypesController : Controller
{
    private readonly MyContext _context;

    public BenefitTypesController(MyContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var benefits = _context.BenefitTypes.ToList();
        return View(benefits);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BenefitType benefit)
    {
        if (ModelState.IsValid)
        {
            _context.BenefitTypes.Add(benefit);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(benefit);
    }

    public IActionResult Edit(int id)
    {
        var benefit = _context.BenefitTypes.Find(id);
        if (benefit == null) return NotFound();
        return View(benefit);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(BenefitType benefit)
    {
        if (ModelState.IsValid)
        {
            _context.BenefitTypes.Update(benefit);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(benefit);
    }

    public IActionResult Delete(int id)
    {
        var benefit = _context.BenefitTypes.Find(id);
        if (benefit != null)
        {
            _context.BenefitTypes.Remove(benefit);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}
