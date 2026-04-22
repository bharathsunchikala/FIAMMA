using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Fiamma.Infrastructure.Identity;
using Fiamma.PublicApi.Extensions;

namespace Fiamma.PublicApi.UserManagementEndpoints;

public class UserListEndpoint(UserManager<ApplicationUser> userManager):EndpointWithoutRequest<UserListResponse>
{

    public override void Configure()
    {
        Get("api/users");
        Roles(BlazorShared.Authorization.Constants.Roles.ADMINISTRATORS);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(d => d.Produces<UserListResponse>()
        .WithTags("UserManagementEndpoints"));
    }

    public override async Task<UserListResponse> ExecuteAsync(CancellationToken ct)
    {
        await Task.Delay(1000, ct);
        var response = new UserListResponse();
        var users = userManager.Users.ToList();
        foreach ( var user in users)
        {
            response.Users.Add(user.ToUserDto());
        }
        return response;
    }
}

