using AutoMapper;

using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

using ThreeThings.Data.Dto.TodoItemDto;
using ThreeThings.Data.Models;

namespace ThreeThings.Data;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap(typeof(JsonPatchDocument<>), typeof(JsonPatchDocument<>));
        CreateMap(typeof(Operation<>), typeof(Operation<>));
        
        CreateMap<TodoItem, TodoItemViewDto>();
        CreateMap<TodoItemAddDto, TodoItem>();
        CreateMap<TodoItemUpdateDto, TodoItem>();
        CreateMap<TodoItem, TodoItemUpdateDto>();
    }
}
