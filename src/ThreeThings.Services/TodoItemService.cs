using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

using ThreeThings.Data;
using ThreeThings.Data.Models;
using ThreeThings.Utils.Common;
using ThreeThings.Utils.Common.Response;

namespace ThreeThings.Services;

public class TodoItemService : IBasicService
{
    private readonly ILogger<TodoItemService> _logger;

    private readonly ThreeThingsDbContext _dbContext;

    private DbSet<TodoItem> DbSet => _dbContext.TodoItems;

    public TodoItemService(ILogger<TodoItemService> logger, ThreeThingsDbContext dbContext)
    {
        _logger    = logger;
        _dbContext = dbContext;
    }

    public Task<bool> ExistAsync(long id, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(item => item.Id == id, cancellationToken);
    }

    public Task<TodoItem?> FindAsync(long id, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }
        return query.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public async Task<TodoItem> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        return await FindAsync(id, cancellationToken: cancellationToken) ?? throw new BasicException(ResponseCode.EntityNotFound);
    }

    public async Task<(List<TodoItem> Data, int TotalCount)> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var totalCount = await DbSet.CountAsync(cancellationToken);
        var data       = await DbSet.ToListAsync(cancellationToken);
        return (data, totalCount);
    }

    public async Task<EntityEntry<TodoItem>> AddAsync(
        TodoItem item,
        CancellationToken cancellationToken = default)
    {
        var entry = await DbSet.AddAsync(item, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("TodoItem Insert: {ID}", entry.Entity.Id);

        return entry;
    }

    public async Task<EntityEntry<TodoItem>> UpdateAsync(
        TodoItem item,
        CancellationToken cancellationToken = default)
    {
        var entry = _dbContext.Update(item);
        _dbContext.ChangeTracker.DetectChanges();
        Console.WriteLine(_dbContext.ChangeTracker.DebugView.LongView);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
        {
            _logger.LogInformation("TodoItem Update: {ID}", entry.Entity.Id);
        }
        return entry;
    }

    public async Task CompleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var item = await GetAsync(id, cancellationToken);
        if (item.Completed)
        {
            return;
        }
        item.Completed     = true;
        item.CompletedTime = DateTime.Now;
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("TodoItem completed: id = {Id}", id);
    }

    public async Task RestoreAsync(long id, CancellationToken cancellationToken = default)
    {
        var item = await GetAsync(id, cancellationToken);
        if (!item.Completed)
        {
            return;
        }
        item.Completed     = false;
        item.CompletedTime = null;
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("TodoItem restored: id = {Id}", id);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var item = await FindAsync(id, true,  cancellationToken);
        if (item != null)
        {
            item.IsDelete = true;
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("TodoItem deleted: id = {Id}", id);
        }
    }
}
