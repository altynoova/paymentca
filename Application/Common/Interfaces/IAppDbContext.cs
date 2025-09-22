using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DatabaseFacade Database { get; }
        DbSet<Domain.Entities.Payment> Payments { get; }
        DbSet<LoginAttempt> LoginAttempts { get; }
        DbSet<Session> Sessions { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<decimal?> TryDebitBalanceAsync(int userId, decimal amountCents, CancellationToken ct);

    }
}
