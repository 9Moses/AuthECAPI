using System.Security.Claims;
using AuthECAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AuthECAPI.Controllers{

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/userProfile", GetUserProfile);
        return app;
    }

    [Authorize]
    private static async Task<IResult> GetUserProfile(ClaimsPrincipal user, UserManager<ApiUser> userManager)
    {
        string userID = user.Claims.First(x => x.Type == "UserID").Value;
        var userDatils = await userManager.FindByIdAsync(userID);

        return Results.Ok(
            new
            {
                Email = userDatils?.Email,
                FullName = userDatils?.FullName
            });
    }
}
}