namespace Fiamma.PublicApi.UserManagementEndpoints;

public class GetByUserNameUserRequest : BaseRequest
{
    public string UserName { get; init; }

    public GetByUserNameUserRequest(string userName)
    {
        UserName = userName;
    }
}

