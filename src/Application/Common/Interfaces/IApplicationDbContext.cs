using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<MainNumber> MainNumbers { get; }
    public DbSet<AdditionalNumber> AdditionalNumbers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken);
    Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken);
}
