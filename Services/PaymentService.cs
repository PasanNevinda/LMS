using LMS.Data;
using LMS.Helpers;
using LMS.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace LMS.Services
{
    public interface IPaymentService
    {
        Task<(bool success, string message)> PurchaseCourseAsync(string studentId, int courseId, string paymentMethodToken);
    }

    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _db;
        private readonly IPaymentGateway _gateway;
        private readonly decimal _commissionRate; // e.g., 0.15m

        public PaymentService(ApplicationDbContext db, IPaymentGateway gateway, IConfiguration config)
        {
            _db = db;
            _gateway = gateway;
            _commissionRate = config.GetValue<decimal>("Payments:CommissionRate", 0.15m);
        }

        public async Task<(bool success, string message)> PurchaseCourseAsync(string studentId, int courseId, string paymentMethodToken)
        {
            // Basic validations
            var course = await _db.Courses.Include(c => c.Teacher).FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null) return (false, "Course not found.");

            // check if student already enrolled
            var already = await _db.Enrollments.AnyAsync(e => e.CourseId == courseId && e.StudentId == studentId);
            if (already) return (false, "Already enrolled.");

            var amount = Money.Round(course.Price);
            // Charge via gateway
            var res = await _gateway.ChargeAsync(amount, "$", $"Enroll {course.Name}", paymentMethodToken);
            if (!res.Success) return (false, $"Payment failed: {res.Message}");

            // Calculate splits
            var platformFee = Money.Round(amount * _commissionRate);
            var instructorShare = Money.Round(amount - platformFee);

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var payment = new PaymentTransaction
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    Amount = amount,
                    CommissionAmount = platformFee,
                    TransactionId = res.TransactionId,
                    PaymentTime = DateTime.UtcNow,
                };
                _db.PaymentTransactions.Add(payment);
                await _db.SaveChangesAsync();

                course.Teacher.AvailableBalance += instructorShare;
                course.Teacher.LifetimeEarnings += instructorShare;
                await _db.SaveChangesAsync();

                var enrollment = new Enrollment
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    PaymentTransactionId = payment.Id,
                    EnrolledTime = DateTime.UtcNow
                };
                _db.Enrollments.Add(enrollment);

                var finance = await _db.PlatformFinances.FirstOrDefaultAsync(p => p.Id == 1);
                if (finance == null)
                {
                    finance = new PlatformFinance { Id = 1, TotalCommissionEarned = platformFee, TotalPaidToInstructors = 0m };
                    _db.PlatformFinances.Add(finance);
                }
                else
                {
                    finance.TotalCommissionEarned = Money.Round(finance.TotalCommissionEarned + platformFee);
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return (true, "Enrolled and payment recorded.");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return (false, ex.Message);
            }
        }
    }

}
