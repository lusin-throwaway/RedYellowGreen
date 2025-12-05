using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RedYellowGreen.Api.Infrastructure.Database.Models;

namespace RedYellowGreen.Api.Infrastructure.Database.Interceptors;

internal sealed class AuditableFieldsInterceptor : SaveChangesInterceptor
{
    private readonly TimeProvider _timeProvider;

    public AuditableFieldsInterceptor(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        PopulateTimestamps(eventData);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        PopulateTimestamps(eventData);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void PopulateTimestamps(DbContextEventData eventData)
    {
        if (eventData?.Context?.ChangeTracker is not { } changeTracker)
            return;

        var now = _timeProvider.GetUtcNow();
        changeTracker
            .Entries<BaseEntity>()
            .Where(e => e.State is EntityState.Added)
            .ToList()
            .ForEach(e =>
            {
                // if it's explicitly set - don't overwrite it
                if (e.Entity.CreatedAt == DateTime.MinValue)
                    e.Entity.CreatedAt = now.UtcDateTime;
            });
    }
}