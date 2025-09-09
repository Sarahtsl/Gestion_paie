using Gestion_paie.DataBase;
using Gestion_paie.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index()
        {
            var payrolls = await _context.Payrolls
                .Include(p => p.Employee)
                .Include(p => p.PayrollPeriod)
                .ToListAsync();

            return View(payrolls);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Employees = await _context.Employees.ToListAsync();
            ViewBag.Periods = await _context.PayrollPeriods.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int employeeId, int periodId, decimal overtimeHours, decimal bonusAmount)
        {
            var payroll = await _payrollService.GeneratePayrollAsync(employeeId, periodId, overtimeHours, bonusAmount);

            TempData["Message"] = $"Fiche de paie générée pour {payroll.Employee.FirstName} {payroll.Employee.LastName}";
            return RedirectToAction(nameof(Index));
        }
    }
}