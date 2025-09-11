using Gestion_paie.DataBase;
using Gestion_paie.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

namespace Gestion_paie.Controllers
{
    public class PayrollController : Controller
    {
        private readonly MyContext _context;
        private readonly PayrollService _payrollService;

        public PayrollController(MyContext context)
        {
            _context = context;
            _payrollService = new PayrollService(_context);
        }

        public IActionResult Index(int? periodId)
        {
            ViewBag.Periods = _context.PayrollPeriods
                                      .OrderByDescending(p => p.PeriodYear)
                                      .ThenByDescending(p => p.PeriodMonth)
                                      .ToList();

            ViewBag.SelectedPeriodId = periodId;

            var payrolls = _context.Payrolls
                                   .Include(p => p.Employee)
                                   .Include(p => p.PayrollPeriod)
                                   .AsQueryable();

            if (periodId.HasValue)
            {
                payrolls = payrolls.Where(p => p.PeriodId == periodId.Value);
            }

            return View(payrolls.ToList());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Employees = await _context.Employees.ToListAsync();
            ViewBag.Periods = await _context.PayrollPeriods.ToListAsync();
            ViewBag.BenefitTypes = await _context.BenefitTypes.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int employeeId,
            int periodId,
            decimal overtimeHours,
            decimal bonusAmount,
            List<int> benefitTypeIds,
            List<decimal> values,
            List<decimal> taxableValues,
            List<string> descriptions)  
        {
            var payroll = await _payrollService.GeneratePayrollAsync(employeeId, periodId, overtimeHours, bonusAmount);

            if (benefitTypeIds != null && benefitTypeIds.Count > 0)
            {
                await _payrollService.AddBenefitsInKindAsync(payroll.Id, benefitTypeIds, values, taxableValues, descriptions);
            }

            TempData["Message"] = $"Fiche de paie générée pour {payroll.Employee.FirstName} {payroll.Employee.LastName}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var payroll = _context.Payrolls
                                  .Include(p => p.PayrollPeriod)
                                  .Include(p => p.Employee)
                                  .FirstOrDefault(p => p.Id == id);

            if (payroll == null)
            {
                TempData["Message"] = "Fiche de paie introuvable.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Payrolls.Remove(payroll);
                _context.SaveChanges();
                TempData["Message"] = "Fiche de paie supprimée avec succès.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Erreur lors de la suppression : {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddBenefits(
            int payrollId,
            List<int> benefitTypeIds,
            List<decimal> values,
            List<decimal> taxableValues,
            List<string> descriptions)   // <-- Ajouter descriptions ici aussi
        {
            try
            {
                await _payrollService.AddBenefitsInKindAsync(payrollId, benefitTypeIds, values, taxableValues, descriptions);
                return RedirectToAction("PayrollDetails", new { id = payrollId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("PayrollDetails", new { id = payrollId });
            }
        }

        public IActionResult DownloadPdf(int id)
        {
            var payroll = _context.Payrolls
                .Include(p => p.Employee)
                .Include(p => p.PayrollPeriod)
                .Include(p => p.Benefits)
                    .ThenInclude(b => b.BenefitType)
                .FirstOrDefault(p => p.Id == id);

            if (payroll == null)
                return NotFound("Fiche de paie introuvable.");

            string employeeName = $"{payroll.Employee.FirstName}_{payroll.Employee.LastName}";
            string period = $"{payroll.PayrollPeriod.PeriodMonth:D2}-{payroll.PayrollPeriod.PeriodYear}";
            string fileName = $"FichePaie_{employeeName}_{period}.pdf";

            return new ViewAsPdf("PayrollPdf", payroll)
            {
                FileName = fileName,
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageMargins = new Rotativa.AspNetCore.Options.Margins(20, 20, 20, 20)
            };
        }
    }
}
