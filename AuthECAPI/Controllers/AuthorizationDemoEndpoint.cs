using Microsoft.AspNetCore.Authorization;

namespace AuthECAPI.Controllers;

public static class AuthorizationDemoEndpoint
{
    public static IEndpointRouteBuilder MapAuthorizationDemoEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/adminOnly", AdminOnly);
        
        app.MapGet("/adminOrteacher",[Authorize(Roles = "Admin,Teacher")] () =>
        {
            return "Admin or Teacher";
        }); 
        
        app.MapGet("/libraryMemberOnly",[Authorize(Policy = "HasLibraryID")] () =>
        {
            return "library Member Only";
        });
        
        app.MapGet("/applyForMaternityLeave",[Authorize(Roles = "Teacher" ,Policy = "FemalesOnly")] () =>
        {
            return "Applied for maternity leave";
        }); 
       
        app.MapGet("/applyForMaternityLeave",[Authorize(Policy = "Under10")] [Authorize(Policy = "FemalesOnly")] () =>
        {
            return "Applied for maternity leave";
        });
        
        return app;
    }
    
    [Authorize(Roles = "Admin")]
    private static string AdminOnly()
    {
        return "Admin Only";
    }
}
