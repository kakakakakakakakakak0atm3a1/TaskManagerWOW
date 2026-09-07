using AutoMapper;
using TaskManager.Api.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Api.Mapping;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TaskItem, TaskResponseDto>();
        CreateMap<CreateTaskDto, TaskItem>();
    }
            
}