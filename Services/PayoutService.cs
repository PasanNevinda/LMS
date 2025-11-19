using LMS.Data;
using LMS.Helpers;
using LMS.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace LMS.Services
{
    public interface IPayoutService
    {
        Task<decimal> GetTeacherBalanceAsync(string teacherId);
        Task<(bool success, string message)> CreatePayoutRequestAsync(string teacherId, decimal amount, string BankAccountNo);
        Task<(bool success, string message)> ApprovePayoutAsync(int payoutId, string adminId);
    }

    public class PayoutService : IPayoutService
    {
        private readonly ApplicationDbContext _db;
        public PayoutService(ApplicationDbContext db) { _db = db; }


        public async Task<decimal> GetTeacherBalanceAsync(string teacherId)
        {
            var teacher = await _db.Teachers.FindAsync(teacherId);
            return teacher?.AvailableBalance ?? 0m;
        }
         

        public async Task<(bool success, string message)> CreatePayoutRequestAsync(string teacherId, decimal amount, string BankAccountNo)
        {
            var teacher = await _db.Teachers.FindAsync(teacherId);
            if (teacher == null) return (false, "Teacher not found");
            if (amount <= 0) return (false, "Invalid amount");
            if (amount > teacher.AvailableBalance) return (false, "Insufficient balance");


            var payout = new InstructorPayout
            {
                TeacherId = teacherId,
                Amount = amount,
                RequestedAt = DateTime.UtcNow,
                Status = PayoutStatus.Pending,
                BankAccountNo = BankAccountNo
            };


            _db.InstructorPayouts.Add(payout);
            await _db.SaveChangesAsync();


            return (true, "Payout request submitted.");
        }


        public async Task<(bool success, string message)> ApprovePayoutAsync(int payoutId, string adminId)
        {
            var payout = await _db.InstructorPayouts.Include(p => p.Teacher).FirstOrDefaultAsync(p => p.Id == payoutId);
            if (payout == null) return (false, "Not found");
            if (payout.Status != PayoutStatus.Pending) return (false, "Invalid state");


            if (payout.Amount > payout.Teacher.AvailableBalance)
                return (false, "Insufficient balance");


            payout.Teacher.AvailableBalance -= payout.Amount;

            var finance = await _db.PlatformFinances.FirstOrDefaultAsync(p => p.Id == 1);
            if (finance != null)
            {
                finance.TotalPaidToInstructors = Money.Round(finance.TotalPaidToInstructors + payout.Amount);
            }
            await _db.SaveChangesAsync();

            payout.Status = PayoutStatus.Paid;
            payout.PaidAt = DateTime.UtcNow;


            await _db.SaveChangesAsync();
            return (true, "Payout approved. Perform manual bank transfer.");
        }

    }

    //public class PayoutService : IPayoutService
    //{
    //    private readonly ApplicationDbContext _db;

    //    public PayoutService(ApplicationDbContext db)
    //    {
    //        _db = db;
    //    }

    //    public async Task<(bool success, string message)> CreatePayoutAsync(int teacherId, decimal amount, int adminUserId, string adminNote = null)
    //    {
    //        // 1. Verify teacher exists
    //        var teacher = await _db.Teachers.FindAsync(teacherId);
    //        if (teacher == null) return (false, "Teacher not found.");
    //        if (amount <= 0) return (false, "Invalid amount.");

    //        // 2. Check available balance
    //        var available = await GetTeacherAvailableBalanceAsync(teacherId);
    //        if (amount > available) return (false, $"Requested amount ({amount}) is greater than available ({available}).");

    //        using var tx = await _db.Database.BeginTransactionAsync();
    //        try
    //        {
    //            // 3. Create InstructorPayout record (Pending -> Paid once transferred)
    //            var payout = new InstructorPayout
    //            {
    //                TeacherId = teacherId,
    //                Amount = Money.Round(amount),
    //                RequestedAtUtc = DateTime.UtcNow,
    //                Status = PayoutStatus.Paid, // since admin will do a manual bank transfer immediately; you can set Pending then Paid after transfer
    //                PaidAtUtc = DateTime.UtcNow,
    //                AdminNote = adminNote
    //            };
    //            _db.InstructorPayouts.Add(payout);

    //            // 4. Mark PaymentTransactions as settled until amount allocated
    //            decimal remaining = amount;
    //            var unsettledTxs = await _db.PaymentTransactions
    //                .Where(pt => pt.Course.TeacherId == teacherId && pt.Status == PaymentStatus.Completed && !pt.InstructorShareSettled)
    //                .OrderBy(pt => pt.PaymentDateUtc)
    //                .ToListAsync();

    //            foreach (var pt in unsettledTxs)
    //            {
    //                if (remaining <= 0) break;

    //                // If pt.InstructorShare <= remaining, include whole tx
    //                if (pt.InstructorShare <= remaining)
    //                {
    //                    pt.InstructorShareSettled = true;
    //                    payout.PaymentTransactions.Add(pt);
    //                    remaining = Money.Round(remaining - pt.InstructorShare);
    //                }
    //                else
    //                {
    //                    // Partially consume a transaction:
    //                    // Option A: Do not partially settle individual transactions (simpler).
    //                    // Option B: Track partial settled amount (requires adding fields).
    //                    // We'll choose Option A here: only settle full transactions. Prevent payouts that would require partial consumption.
    //                    // So if there is one large transaction, disallow payout until exact match or change approach.
    //                    // For simplicity, stop here and throw.
    //                    throw new InvalidOperationException("Partial transaction settlement required; not supported in this implementation. Increase available to include whole transactions or implement partial settlement.");
    //                }
    //            }

    //            if (remaining != 0)
    //            {
    //                // Not able to allocate entire amount via full transactions
    //                throw new InvalidOperationException("Unable to allocate exact amount from unsettled transactions (partial settlement not implemented).");
    //            }

    //            // 5. Update PlatformFinance totals (TotalPaidToInstructors)
    //            var finance = await _db.PlatformFinances.FirstOrDefaultAsync(p => p.Id == 1);
    //            if (finance == null) throw new InvalidOperationException("Platform finance row missing.");
    //            finance.TotalPaidToInstructors = Money.Round(finance.TotalPaidToInstructors + amount);

    //            await _db.SaveChangesAsync();
    //            await tx.CommitAsync();

    //            // TODO: Admin performs real bank transfer using teacher.BankName/AccountNumber outside system.
    //            // Save admin bank transfer reference in payout.AdminNote/ExternalPayoutReference if needed.

    //            return (true, "Payout recorded as paid. Please perform manual bank transfer using teacher's bank details.");
    //        }
    //        catch (Exception ex)
    //        {
    //            await tx.RollbackAsync();
    //            return (false, ex.Message);
    //        }
    //    }
    //}

}
