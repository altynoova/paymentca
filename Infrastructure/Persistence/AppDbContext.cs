using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Reflection;

namespace Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public class Role : Microsoft.AspNetCore.Identity.IdentityRole<int> { }
        public async Task<decimal?> TryDebitBalanceAsync(int userId, decimal amountCents, CancellationToken ct)
        {
            await using var tx = await Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);

            decimal? newBalance = null;
            await using (var cmd = Database.GetDbConnection().CreateCommand())
            {
                cmd.Transaction = Database.CurrentTransaction?.GetDbTransaction();
                cmd.CommandText = """
                    UPDATE "AspNetUsers"
                    SET "balance_cents" = "balance_cents" - @p0
                    WHERE "id" = @p1 AND "balance_cents" >= @p0
                    RETURNING "balance_cents";
                """;
                var p0 = cmd.CreateParameter(); p0.ParameterName = "p0"; p0.Value = amountCents;
                var p1 = cmd.CreateParameter(); p1.ParameterName = "p1"; p1.Value = userId;
                cmd.Parameters.Add(p0); cmd.Parameters.Add(p1);

                if (cmd.Connection!.State != ConnectionState.Open)
                    await Database.OpenConnectionAsync(ct);

                var result = await cmd.ExecuteScalarAsync(ct);
                if (result != null) newBalance = (decimal)result;
            }

            if (newBalance is null) { await tx.RollbackAsync(ct); return null; }
            await tx.CommitAsync(ct);
            return newBalance;
        }
    }
}
