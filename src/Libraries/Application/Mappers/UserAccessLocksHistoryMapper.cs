using Application.DataTransfareObjects.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers
{
    public class UserAccessLocksHistoryMapper : Profile
    {
        public UserAccessLocksHistoryMapper()
        {
            CreateMap<LockAccessHistoryEntity, UserAccessHistortLocksResponseDto>()
                .ForMember(x => x.LastAccessed, y => y.MapFrom(s => s.ModifiedAt ?? s.CreatedAt))
                .ForMember(x => x.FloorIndex, y => y.MapFrom(s => s.DoorLock.Floor.Index))
                .ForMember(x => x.DoorName, y => y.MapFrom(s => s.DoorLock.DoorName))
                .ForMember(x => x.BuildingName, y => y.MapFrom(s => s.DoorLock.Floor.Building.Name));
        }
    }
}
