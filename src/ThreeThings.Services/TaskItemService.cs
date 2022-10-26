using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

using ThreeThings.Data;
using ThreeThings.Data.Models;
using ThreeThings.Utils.Common;
using ThreeThings.Utils.Common.Response;

namespace ThreeThings.Services;

public class TaskItemService : IBasicService
{
    private readonly ILogger<TaskItemService> _logger;

    private readonly ThreeThingsDbContext _dbContext;

    private DbSet<TaskItem> DbSet => _dbContext.TaskItems;

    public TaskItemService(ILogger<TaskItemService> logger, ThreeThingsDbContext dbContext)
    {
        _logger    = logger;
        _dbContext = dbContext;
    }

    public Task<bool> ExistAsync(long id, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(item => item.Id == id, cancellationToken);
    }

    public Task<TaskItem?> FindAsync(long id, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();
        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }
        return query.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public async Task<TaskItem> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        return await FindAsync(id, cancellationToken: cancellationToken) ?? throw new BasicException(ResponseCode.EntityNotFound);
    }

    public async Task<(List<TaskItem> Data, int TotalCount)> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var totalCount = await DbSet.CountAsync(cancellationToken);
        var data       = await DbSet.ToListAsync(cancellationToken);
        return (data, totalCount);
    }

    public async Task<EntityEntry<TaskItem>> AddAsync(
        TaskItem item,
        CancellationToken cancellationToken = default)
    {
        var entry = await DbSet.AddAsync(item, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("TaskItem Insert: {ID}", entry.Entity.Id);

        return entry;
    }

    public async Task<EntityEntry<TaskItem>> UpdateAsync(
        TaskItem item,
        CancellationToken cancellationToken = default)
    {
        var entry = _dbContext.Update(item);
        _dbContext.ChangeTracker.DetectChanges();
        Console.WriteLine(_dbContext.ChangeTracker.DebugView.LongView);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
        {
            _logger.LogInformation("TaskItem Update: {ID}", entry.Entity.Id);
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

        _logger.LogInformation("TaskItem completed: id = {Id}", id);
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

        _logger.LogInformation("TaskItem restored: id = {Id}", id);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var item = await FindAsync(id, true,  cancellationToken);
        if (item != null)
        {
            item.IsDelete = true;
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("TaskItem deleted: id = {Id}", id);
        }
    }
}
