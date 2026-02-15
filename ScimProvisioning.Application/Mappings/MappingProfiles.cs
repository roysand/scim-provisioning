using AutoMapper;
using ScimProvisioning.Application.DTOs;
using ScimProvisioning.Core.Entities;

namespace ScimProvisioning.Application.Mappings;

/// <summary>
/// AutoMapper profile for user mappings
/// </summary>
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<ScimUser, UserResponse>();
        CreateMap<CreateUserRequest, ScimUser>();
    }
}

/// <summary>
/// AutoMapper profile for group mappings
/// </summary>
public class GroupMappingProfile : Profile
{
    public GroupMappingProfile()
    {
        CreateMap<ScimGroup, GroupResponse>();
        CreateMap<GroupMember, GroupMemberDto>();
        CreateMap<CreateGroupRequest, ScimGroup>();
    }
}
