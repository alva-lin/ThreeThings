using ThreeThings.Utils.Common.Entities;

namespace ThreeThings.Data.Dto.TodoItemDto;

public class TodoItemViewDto : AuditEntity<long>
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 是否完成
    /// </summary>
    public bool Completed { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    public DateTime? CompletedTime { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}
