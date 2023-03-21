using AutoMapper;
using GeoChat.Identity.Api.Dtos;
using GeoChat.Identity.Api.Entities;
using GeoChat.Identity.Api.MessageQueue.Events;

namespace GeoChat.Identity.Api.MappingProfiles
{
    public class UserMappings : Profile
    {
        public UserMappings()
        {
            CreateMap<AppUser, UserResponseDto>();
            CreateMap<AppUser, NewAccountCreatedEvent>();
            CreateMap<UserRegisterDto, AppUser>();
        }
    }
}
