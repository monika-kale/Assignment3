using AutoMapper;
using WebApplication1.Models;  // Adjust namespace accordingly

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // Map User -> UserDto
        CreateMap<User, UserDto>();

        // If you need reverse mapping (UserDto -> User)
        // CreateMap<UserDto, User>();
    }
}
