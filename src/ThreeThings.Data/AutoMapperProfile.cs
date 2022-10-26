using AutoMapper;

using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

using ThreeThings.Data.Dto.TaskItemDto;
using ThreeThings.Data.Models;

namespace ThreeThings.Data;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap(typeof(JsonPatchDocument<>), typeof(JsonPatchDocument<>));
        CreateMap(typeof(Operation<>), typeof(Operation<>));
        
        CreateMap<TaskItem, TaskItemViewDto>();
        CreateMap<TaskItemAddDto, TaskItem>();
        CreateMap<TaskItemUpdateDto, TaskItem>();
        CreateMap<TaskItem, TaskItemUpdateDto>();
    }
}
