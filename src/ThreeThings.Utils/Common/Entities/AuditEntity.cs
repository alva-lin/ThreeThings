// ReSharper disable RedundantExtendsListEntry
namespace ThreeThings.Utils.Common.Entities;

public abstract class AuditEntity<TKey> : BasicEntity<TKey>, IBasicEntity<TKey>, IAuditable where TKey : IEquatable<TKey>
{
    /// <summary>
    /// 创建人
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 修改人
    /// </summary>
    public string ModifiedBy { get; set; } = string.Empty;

    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTime? ModifiedTime { get; set; }
}
