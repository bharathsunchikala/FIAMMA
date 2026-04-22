namespace Fiamma.PublicApi.UserManagementEndpoints;

public class UpdateUserRequest : BaseRequest
{
    public UserDto User { get; set; }
}

