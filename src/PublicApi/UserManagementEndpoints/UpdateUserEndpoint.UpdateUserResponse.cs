using System;
using Fiamma.Infrastructure.Identity;

namespace Fiamma.PublicApi.UserManagementEndpoints;

public class UpdateUserResponse : BaseResponse
{
    public UpdateUserResponse(Guid correlationId) : base(correlationId)
    {
    }

    public UpdateUserResponse()
    {
    }
    public ApplicationUser User { get; set; }
}

