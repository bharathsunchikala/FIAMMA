using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Fiamma.Infrastructure.Identity;
using Fiamma.PublicApi.Extensions;
using Fiamma.PublicApi.UserManagementEndpoints.Models;

namespace Fiamma.PublicApi.UserManagementEndpoints;

public class UserGetByIdEndpoint (UserManager<ApplicationUser> userManager) : Endpoint <GetByIdUserRequest, Results<Ok<GetUserResponse>,NotFound>>
{
    public override void Configure()
    {
        Get("api/users/{userId}");
        Roles(BlazorShared.Authorization.Constants.Roles.ADMINISTRATORS);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(d =>
            d.Produces<GetUserResponse>()
            .WithTags("UserManagementEndpoints"));
    }

    public override async Task<Results<Ok<GetUserResponse>, NotFound>> ExecuteAsync(GetByIdUserRequest request, CancellationToken ct)
    {
        var response = new GetUserResponse(request.CorrelationId());

        var userResponse = await userManager.FindByIdAsync(request.UserId);
        if (userResponse is null)
        {
            return TypedResults.NotFound();
        }
        response.User = userResponse.ToUserDto();
        return TypedResults.Ok(response);
    }
}

