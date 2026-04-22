namespace Fiamma.PublicApi.UserManagementEndpoints;

public class CreateUserRequest : BaseRequest
{
    public UserDto User { get; set; }
}

