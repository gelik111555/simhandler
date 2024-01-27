using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<MainNumber> MainNumbers => Set<MainNumber>();
    public DbSet<AdditionalNumber> AdditionalNumbers => Set<AdditionalNumber>();
    public async Task<IDbContextTransaction> BeginTransactionAsync
        (CancellationToken cancellationToken)
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }
    public async Task CommitTransactionAsync
        (IDbContextTransaction transaction, CancellationToken cancellationToken)
    {
        await transaction.CommitAsync(cancellationToken);
    }
    public async Task RollbackTransactionAsync
        (IDbContextTransaction transaction, CancellationToken cancellationToken)
    {
        await transaction.RollbackAsync(cancellationToken);
    }
}
