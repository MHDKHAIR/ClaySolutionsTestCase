using Application.DataTransfareObjects.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers
{
    public class UserLockClaimMapper : Profile
    {
        public UserLockClaimMapper()
        {
            CreateMap<UserLockClaimEntity, UserLocksResponseDto>()
                .ForMember(x => x.LastAccessed, y => y.MapFrom(s => s.ModifiedAt ?? s.CreatedAt))
                .ForMember(x => x.DoorName, y => y.MapFrom(s => s.DoorLock.DoorName))
                .ForMember(x => x.DoorKeyCode, y => y.MapFrom(s => s.DoorLock.DoorKeyCode))
                .ForMember(x => x.FloorIndex, y => y.MapFrom(s => s.DoorLock.Floor.Index))
                .ForMember(x => x.BuildingName, y => y.MapFrom(s => s.DoorLock.Floor.Building.Name));
        }
    }
}
