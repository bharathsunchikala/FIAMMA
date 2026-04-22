using System;
using Microsoft.AspNetCore.Identity;

namespace Fiamma.PublicApi.RoleManagementEndpoints;

public class UpdateRoleResponse : BaseResponse
{
    public UpdateRoleResponse(Guid correlationId) : base(correlationId)
    {
    }

    public UpdateRoleResponse()
    {
    }
    public IdentityRole Role { get; set; }
}

