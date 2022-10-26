using System.ComponentModel.DataAnnotations;

namespace ThreeThings.Data.Dto.TaskItemDto;

public class TaskItemAddDto
{
    /// <summary>
    /// 标题
    /// </summary>
    [Required]
    public string Title { get; set; } = null!;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}
