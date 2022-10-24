#pragma warning disable CS8618
namespace ThreeThings.Utils.Common.Entities;

public abstract class BasicEntity<TKey> : IBasicEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
}
