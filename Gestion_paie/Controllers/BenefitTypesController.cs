using Microsoft.AspNetCore.Mvc;
using GestionPaie.Models;
using Gestion_paie.DataBase;

public class BenefitTypeController : Controller
{
    private readonly MyContext _context;

    public BenefitTypeController(MyContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BenefitType model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
            return Json(new { success = false, errors = new[] { "Le nom est requis." } });

        var benefitType = new BenefitType { Name = model.Name };
        _context.BenefitTypes.Add(benefitType);
        await _context.SaveChangesAsync();

        return Json(new { success = true, id = benefitType.Id, name = benefitType.Name });
    }
}
