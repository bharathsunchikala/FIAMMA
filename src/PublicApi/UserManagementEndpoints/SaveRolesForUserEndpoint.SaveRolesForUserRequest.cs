using System.Collections.Generic;

namespace Fiamma.PublicApi.UserManagementEndpoints;

public class SaveRolesForUserRequest : BaseRequest
{
    public string UserId { get; set; }
    public List<string> RolesToAdd { get; set; } = [];
    public List<string> RolesToRemove { get; set; } = [];
}

