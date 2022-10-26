using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using ThreeThings.Utils.Common.Entities;
using ThreeThings.Utils.Extensions;
using ThreeThings.Data.Models;

#pragma warning disable CS8618

namespace ThreeThings.Data;

public class ThreeThingsDbContext : DbContext
{
    private const string APP_USER = "Server";
    private readonly string _userName;

    public ThreeThingsDbContext(DbContextOptions<ThreeThingsDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {

        var claimsPrincipal = httpContextAccessor.HttpContext?.User;
        _userName = claimsPrincipal?.Claims.SingleOrDefault(c => c.Type == "username")?.Value ?? APP_USER;
    }

    #region DbSet

    public virtual DbSet<TaskItem> TaskItems { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyUtcDateTimeConverter()
            .ApplySoftDeleteGlobalQueryFilter();
    }

    public override int SaveChanges()
    {
        BeforeSaveChanges();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        BeforeSaveChanges();
        return await base.SaveChangesAsync(cancellationToken);
    }

    #region Helper

    private void BeforeSaveChanges()
    {
        SetPartialUpdate();
        SetAuditInfo();
        SetSoftDelete();
    }

    private void SetPartialUpdate()
    {
        var modifiedEntries = ChangeTracker.Entries()
            .Where(entry => entry.State == EntityState.Modified)
            .ToList();

        foreach (var modifiedEntry in modifiedEntries)
        {
            foreach (var propertyEntry in modifiedEntry.Properties.Where(pEntry => pEntry.IsModified))
            {
                if (Equals(propertyEntry.OriginalValue, propertyEntry.CurrentValue))
                {
                    propertyEntry.IsModified = false;
                }
            }
            if (modifiedEntry.Properties.Count(pEntry => pEntry.IsModified) == 0)
            {
                modifiedEntry.State = EntityState.Unchanged;
            }
        }
    }

    private void SetSoftDelete()
    {
        var now = DateTime.Now;
        var deletedEntries = ChangeTracker.Entries()
            .Where(entry => entry.State == EntityState.Modified)
            .ToList();

        foreach (var deletedEntry in deletedEntries)
        {
            if (deletedEntry.Entity is ISoftDelete { IsDelete: true } softDelete &&
                deletedEntry.OriginalValues.GetValue<bool>(nameof(ISoftDelete.IsDelete)) == false)
            {
                softDelete.DeletedBy   = _userName;
                softDelete.DeletedTime = now;
            }
        }
    }

    private void SetAuditInfo()
    {
        var now = DateTime.Now;
        var addedEntities = ChangeTracker.Entries()
            .Where(entry => entry.State == EntityState.Added)
            .ToList();

        foreach (var addedEntity in addedEntities)
        {
            if (addedEntity.Entity is IAuditable auditable)
            {
                auditable.CreatedBy    = _userName;
                auditable.CreationTime = now;
            }
        }

        var modifiedEntries = ChangeTracker.Entries()
            .Where(entry => entry.State == EntityState.Modified)
            .ToList();

        foreach (var modifiedEntry in modifiedEntries)
        {
            if (modifiedEntry.Entity is IAuditable auditable)
            {
                var entry = Entry(auditable);
                entry.Property(e => e.CreatedBy).IsModified    = false;
                entry.Property(e => e.CreationTime).IsModified = false;

                auditable.ModifiedBy   = _userName;
                auditable.ModifiedTime = now;
            }
        }
    }

    #endregion
}
