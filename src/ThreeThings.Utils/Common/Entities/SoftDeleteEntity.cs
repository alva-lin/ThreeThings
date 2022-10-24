// ReSharper disable RedundantExtendsListEntry
namespace ThreeThings.Utils.Common.Entities;

public abstract class SoftDeleteEntity<TKey> : AuditEntity<TKey>, IBasicEntity<TKey>, ISoftDelete where TKey : IEquatable<TKey>
{
    /// <summary>
    /// 是否删除
    /// </summary>
    public bool IsDelete { get; set; }

    /// <summary>
    /// 删除人
    /// </summary>
    public string? DeletedBy { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    public DateTime? DeletedTime { get; set; }
}
