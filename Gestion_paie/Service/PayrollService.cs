using Gestion_paie.DataBase;
using Gestion_paie.Models;
using GestionPaie.Models;
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
        if (employee == null) throw new Exception("Employé introuvable");

        var period = await _context.PayrollPeriods.FindAsync(periodId);
        if (period == null) throw new Exception("Période introuvable");

        var cnssRate = await _context.CnssRates
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.EffectiveDate)
            .FirstOrDefaultAsync();

        if (cnssRate == null) throw new Exception("Aucun taux CNSS défini");

        decimal baseSalary = employee.BaseSalary;
        decimal hourlyRate = baseSalary / 173;
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
          
            BenefitsInKind = 0
        };

        _context.Payrolls.Add(payroll);
        await _context.SaveChangesAsync();

        return payroll;
    }

    public async Task AddBenefitsInKindAsync(int payrollId, List<int> benefitTypeIds, List<decimal> values, List<decimal> taxableValues, List<string> descriptions)
    {
        var payroll = await _context.Payrolls
            .Include(p => p.Benefits)
            .FirstOrDefaultAsync(p => p.Id == payrollId);

        if (payroll == null) throw new Exception("Paie introuvable");

        for (int i = 0; i < benefitTypeIds.Count; i++)
        {
            var benefit = new BenefitInKind
            {
                PayrollId = payrollId,
                BenefitTypeId = benefitTypeIds[i],
                Value = values[i],
                TaxableValue = taxableValues[i],
                Description = descriptions[i],
                CreatedAt = DateTime.UtcNow
            };

            payroll.Benefits.Add(benefit);

            payroll.BenefitsInKind += values[i];
        }

        payroll.GrossSalary += payroll.Benefits.Sum(b => b.TaxableValue);

        payroll.TaxableIncome = payroll.GrossSalary
            - (payroll.CnssFamilyAllowance + payroll.CnssAmo + payroll.CnssAccident + payroll.CnssRetirement);

        payroll.IncomeTax = CalculateIncomeTax(payroll.TaxableIncome);
        payroll.NetSalary = payroll.TaxableIncome - payroll.IncomeTax;

        await _context.SaveChangesAsync();
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

    public async Task<Payroll> UpdatePayrollAsync(
        int payrollId,
        decimal overtimeHours,
        decimal bonusAmount,
        List<int> benefitTypeIds,
        List<decimal> values,
        List<decimal> taxableValues,
        List<string> descriptions)
    {
        var payroll = await _context.Payrolls
            .Include(p => p.Employee)
            .Include(p => p.PayrollPeriod)
            .Include(p => p.Benefits)
            .FirstOrDefaultAsync(p => p.Id == payrollId);

        if (payroll == null)
            throw new Exception("Fiche de paie introuvable");

        payroll.OvertimeHours = overtimeHours;
        payroll.BonusAmount = bonusAmount;

        decimal hourlyRate = payroll.BaseSalary / 173;
        payroll.OvertimeAmount = overtimeHours * hourlyRate * 1.5m;

        payroll.GrossSalary = payroll.BaseSalary + payroll.OvertimeAmount + payroll.BonusAmount;

        _context.BenefitsInKind.RemoveRange(payroll.Benefits);

        payroll.Benefits = new List<BenefitInKind>();
        for (int i = 0; i < benefitTypeIds.Count; i++)
        {
            var benefit = new BenefitInKind
            {
                PayrollId = payroll.Id,
                BenefitTypeId = benefitTypeIds[i],
                Value = values[i],
                TaxableValue = taxableValues[i],
                Description = descriptions[i],
                CreatedAt = DateTime.UtcNow
            };

            payroll.Benefits.Add(benefit);
            payroll.BenefitsInKind += values[i];
        }

        payroll.GrossSalary += payroll.Benefits.Sum(b => b.TaxableValue);

        var cnssRate = await _context.CnssRates
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.EffectiveDate)
            .FirstOrDefaultAsync();

        if (cnssRate != null)
        {
            payroll.CnssFamilyAllowance = payroll.GrossSalary * (cnssRate.FamilyAllowanceRate ?? 0);
            payroll.CnssAmo = payroll.GrossSalary * (cnssRate.AmoRate ?? 0);
            payroll.CnssAccident = payroll.GrossSalary * (cnssRate.AccidentRate ?? 0);
            payroll.CnssRetirement = payroll.GrossSalary * (cnssRate.RetirementRate ?? 0);
        }

        payroll.TaxableIncome = payroll.GrossSalary
            - (payroll.CnssFamilyAllowance + payroll.CnssAmo + payroll.CnssAccident + payroll.CnssRetirement);

        payroll.IncomeTax = CalculateIncomeTax(payroll.TaxableIncome);
        payroll.NetSalary = payroll.TaxableIncome - payroll.IncomeTax;

        await _context.SaveChangesAsync();
        return payroll;
    }

}


