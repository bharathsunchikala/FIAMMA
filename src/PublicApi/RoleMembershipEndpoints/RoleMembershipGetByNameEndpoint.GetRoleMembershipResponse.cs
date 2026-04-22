using System;
using System.Collections.Generic;
using Fiamma.Infrastructure.Identity;

namespace Fiamma.PublicApi.RoleMembershipEndpoints;

public class GetRoleMembershipResponse : BaseResponse
{

    public GetRoleMembershipResponse(Guid correlationId) : base(correlationId)
    {
    }

    public GetRoleMembershipResponse()
    {
    }

    public List<ApplicationUser> RoleMembers { get; set; }
}

