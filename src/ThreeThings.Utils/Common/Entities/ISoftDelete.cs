namespace ThreeThings.Utils.Common.Entities;

public interface ISoftDelete
{
    public bool IsDelete { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedTime { get; set; }
}
