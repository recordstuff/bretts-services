namespace bretts_services.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, DisplayedUser>();
    }
}
