using AutoMapper;

using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

using ThreeThings.Data.Dto.TaskItemDto;
using ThreeThings.Data.Models;
using ThreeThings.Services;
using ThreeThings.Utils.Common;
using ThreeThings.Utils.Common.Response;
using ThreeThings.Utils.Extensions;

namespace ThreeThings.Api.Controllers;

/// <summary>
/// 待办事项接口
/// </summary>
public class TaskItemController : BasicController
{
    private readonly Lazy<TaskItemService> _taskService;

    private readonly IMapper _mapper;

    public TaskItemController(Lazy<TaskItemService> taskService, IMapper mapper)
    {
        _taskService = taskService;
        _mapper          = mapper;
    }

    /// <summary>
    /// 获取待办详情
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ResponseResult<TaskItemViewDto?>> GetAsync(int id, CancellationToken cancellationToken)
    {
        var item = await _taskService.Value.FindAsync(id, cancellationToken: cancellationToken);
        return _mapper.Map<TaskItemViewDto>(item);
    }

    /// <summary>
    /// 获取所有待办事项
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet()]
    public async Task<ResponseListResult<TaskItemViewDto>> GetListAsync(CancellationToken cancellationToken)
    {
        var (data, totalCount) = await _taskService.Value.GetAllAsync(cancellationToken);
        return (_mapper.Map<List<TaskItemViewDto>>(data), totalCount);
    }

    /// <summary>
    /// 新增待办事项
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ResponseResult<TaskItemViewDto?>> AddAsync(
        [FromBody] TaskItemAddDto dto,
        CancellationToken cancellationToken)
    {
        var item  = _mapper.Map<TaskItemAddDto, TaskItem>(dto);
        var entry = await _taskService.Value.AddAsync(item, cancellationToken);
        return _mapper.Map<TaskItemViewDto>(entry.Entity);
    }

    /// <summary>
    /// 更新待办事项
    /// </summary>
    /// <param name="id"></param>
    /// <param name="jsonPatch"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<ResponseResult<TaskItemViewDto?>> UpdateAsync(
        long id,
        [FromBody] JsonPatchDocument<TaskItemUpdateDto> jsonPatch,
        CancellationToken cancellationToken)
    {
        var item = await _taskService.Value.GetAsync(id, cancellationToken);

        var dto = _mapper.Map<TaskItemUpdateDto>(item);
        jsonPatch.ApplyToSafely(dto, ModelState);
        _mapper.Map(dto, item);
        
        var entry = await _taskService.Value.UpdateAsync(item, cancellationToken);
        
        return _mapper.Map<TaskItemViewDto>(entry.Entity);
    }

    /// <summary>
    /// 完成待办事项
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("complete/{id}")]
    public Task CompleteAsync(long id, CancellationToken cancellationToken)
    {
        return _taskService.Value.CompleteAsync(id, cancellationToken);
    }

    /// <summary>
    /// 恢复待办事项（由完成变为未完成）
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("restore/{id}")]
    public Task RestoreAsync(long id, CancellationToken cancellationToken)
    {
        return _taskService.Value.RestoreAsync(id, cancellationToken);
    }

    /// <summary>
    /// 删除待办事项
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        return _taskService.Value.DeleteAsync(id, cancellationToken);
    }
}
