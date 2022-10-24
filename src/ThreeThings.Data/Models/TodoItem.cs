using System.ComponentModel.DataAnnotations;

using ThreeThings.Utils.Common.Entities;

namespace ThreeThings.Data.Models;

public class TodoItem : SoftDeleteEntity<long>
{
    /// <summary>
    /// 标题
    /// </summary>
    [Required]
    [MinLength(4)]
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
