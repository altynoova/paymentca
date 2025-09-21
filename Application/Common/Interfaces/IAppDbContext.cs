using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Payment> Payments { get; }
        DbSet<LoginAttempt> LoginAttempts { get; }
        DbSet<Session> Sessions { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
