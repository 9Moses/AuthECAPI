using Microsoft.AspNetCore.Authorization;

namespace AuthECAPI.Controllers{

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/userProfile", GetUserProfile);
        return app;
    }

    [Authorize]
    private static string GetUserProfile()
    {
        return "User Profile";
    }
}
}