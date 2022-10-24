using AutoMapper;

using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

using ThreeThings.Data.Dto.TodoItemDto;
using ThreeThings.Data.Models;
using ThreeThings.Services;
using ThreeThings.Utils.Common;
using ThreeThings.Utils.Common.Response;
using ThreeThings.Utils.Extensions;

namespace ThreeThings.Api.Controllers;

/// <summary>
/// 待办事项接口
/// </summary>
public class TodoItemController : BasicController
{
    private readonly Lazy<TodoItemService> _todoItemService;

    private readonly IMapper _mapper;

    public TodoItemController(Lazy<TodoItemService> todoItemService, IMapper mapper)
    {
        _todoItemService = todoItemService;
        _mapper          = mapper;
    }

    /// <summary>
    /// 获取待办详情
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ResponseResult<TodoItemViewDto?>> GetAsync(int id, CancellationToken cancellationToken)
    {
        var item = await _todoItemService.Value.FindAsync(id, cancellationToken: cancellationToken);
        return _mapper.Map<TodoItemViewDto>(item);
    }

    /// <summary>
    /// 获取所有待办事项
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet()]
    public async Task<ResponseListResult<TodoItemViewDto>> GetListAsync(CancellationToken cancellationToken)
    {
        var (data, totalCount) = await _todoItemService.Value.GetAllAsync(cancellationToken);
        return (_mapper.Map<List<TodoItemViewDto>>(data), totalCount);
    }

    /// <summary>
    /// 新增待办事项
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ResponseResult<TodoItemViewDto?>> AddAsync(
        [FromBody] TodoItemAddDto dto,
        CancellationToken cancellationToken)
    {
        var item  = _mapper.Map<TodoItemAddDto, TodoItem>(dto);
        var entry = await _todoItemService.Value.AddAsync(item, cancellationToken);
        return _mapper.Map<TodoItemViewDto>(entry.Entity);
    }

    /// <summary>
    /// 更新待办事项
    /// </summary>
    /// <param name="id"></param>
    /// <param name="jsonPatch"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<ResponseResult<TodoItemViewDto?>> UpdateAsync(
        long id,
        [FromBody] JsonPatchDocument<TodoItemUpdateDto> jsonPatch,
        CancellationToken cancellationToken)
    {
        var item = await _todoItemService.Value.GetAsync(id, cancellationToken);

        var dto = _mapper.Map<TodoItemUpdateDto>(item);
        jsonPatch.ApplyToSafely(dto, ModelState);
        _mapper.Map(dto, item);
        
        var entry = await _todoItemService.Value.UpdateAsync(item, cancellationToken);
        
        return _mapper.Map<TodoItemViewDto>(entry.Entity);
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
        return _todoItemService.Value.CompleteAsync(id, cancellationToken);
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
        return _todoItemService.Value.RestoreAsync(id, cancellationToken);
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
        return _todoItemService.Value.DeleteAsync(id, cancellationToken);
    }
}
