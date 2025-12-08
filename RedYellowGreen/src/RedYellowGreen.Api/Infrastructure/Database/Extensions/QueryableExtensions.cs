using Microsoft.EntityFrameworkCore;
using RedYellowGreen.Api.Infrastructure.Database.Models;

namespace RedYellowGreen.Api.Infrastructure.Database.Extensions;

public static class QueryableExtensions
{
    extension<T>(IQueryable<T> source) where T : BaseEntity
    {
        public async Task<T> FindByIdAsync(Guid id) =>
            await source.SingleOrDefaultAsync(x => x.Id == id) ??
            throw new NotFoundException($"{typeof(T)} not found by Id {id}");
    }
    
    extension<T>(IQueryable<T> source)
    {
        public async Task<T> SingleOrNotFoundAsync() =>
            await source
                .SingleOrDefaultAsync() ??
            throw new NotFoundException($"{typeof(T)} not found");
    }
}