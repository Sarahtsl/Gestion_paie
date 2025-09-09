using Gestion_paie.DataBase;
using Gestion_paie.Models;
using Microsoft.EntityFrameworkCore;

public class PayrollService
{
    private readonly MyContext _context;

    public PayrollService(MyContext context)
    {
        _context = context;
    }

    public async Task<Payroll> GeneratePayrollAsync(int employeeId, int periodId, decimal overtimeHours, decimal bonusAmount)
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null)
            throw new Exception("Employé introuvable");

        var period = await _context.PayrollPeriods.FindAsync(periodId);
        if (period == null)
            throw new Exception("Période introuvable");

        // 🔹 Charger le taux CNSS actif
        var cnssRate = await _context.CnssRates
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.EffectiveDate)
            .FirstOrDefaultAsync();

        if (cnssRate == null)
            throw new Exception("Aucun taux CNSS défini");

        // 🔹 Salaire brut de base
        decimal baseSalary = employee.BaseSalary;

        // 🔹 Heures sup (exemple : 1.5x le taux horaire)
        decimal hourlyRate = baseSalary / 173; // base 173h/mois
        decimal overtimeAmount = overtimeHours * hourlyRate * 1.5m;

        decimal grossSalary = baseSalary + overtimeAmount + bonusAmount;

        decimal cnssFamily = grossSalary * (cnssRate.FamilyAllowanceRate ?? 0);
        decimal cnssAmo = grossSalary * (cnssRate.AmoRate ?? 0);
        decimal cnssAccident = grossSalary * (cnssRate.AccidentRate ?? 0);
        decimal cnssRetirement = grossSalary * (cnssRate.RetirementRate ?? 0);

        decimal totalCnss = cnssFamily + cnssAmo + cnssAccident + cnssRetirement;

        decimal taxableIncome = grossSalary - totalCnss;

        decimal incomeTax = CalculateIncomeTax(taxableIncome);

        decimal netSalary = taxableIncome - incomeTax;

        var payroll = new Payroll
        {
            EmployeeId = employee.Id,
            PeriodId = period.Id,
            BaseSalary = baseSalary,
            OvertimeHours = overtimeHours,
            OvertimeAmount = overtimeAmount,
            BonusAmount = bonusAmount,
            GrossSalary = grossSalary,
            CnssFamilyAllowance = cnssFamily,
            CnssAmo = cnssAmo,
            CnssAccident = cnssAccident,
            CnssRetirement = cnssRetirement,
            TaxableIncome = taxableIncome,
            IncomeTax = incomeTax,
            NetSalary = netSalary,
            Status = PayrollStatus.CALCULATED
        };

        _context.Payrolls.Add(payroll);
        await _context.SaveChangesAsync();

        return payroll;
    }

    private decimal CalculateIncomeTax(decimal taxableIncome)
    {
        var brackets = _context.TaxBrackets
            .Where(b => b.IsActive)
            .OrderBy(b => b.MinAmount)
            .ToList();

        foreach (var bracket in brackets)
        {
            if (taxableIncome >= bracket.MinAmount &&
               (bracket.MaxAmount == null || taxableIncome <= bracket.MaxAmount))
            {
                return (taxableIncome - bracket.MinAmount) * (bracket.TaxRate / 100) + bracket.FixedAmount;
            }
        }

        return 0;
    }
}
