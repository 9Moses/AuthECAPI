using System.Text;
using AuthECAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AuthECAPI.Extension;

public static class IdentifyExtension
{
    public static IServiceCollection AddIdentityHandlersAndStore(this IServiceCollection services)
    {
        services
            .AddIdentityApiEndpoints<ApiUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDBContext>();
        return services;
    }

    public static IServiceCollection ConfigureIdentityOptions(this IServiceCollection services)
    {
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.User.RequireUniqueEmail = true;
        });
        
        return services;
    }


    public static IServiceCollection AddIdentityAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme
        ).AddJwtBearer(y =>
        {
            y.SaveToken = false;
            y.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(config["AppSettings:JWTSecret"]!)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
                
            };
        });
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().Build();
            
            options.AddPolicy("HasLibraryID", policy => policy.RequireClaim("LibraryID")); 
            options.AddPolicy("FemalesOnly", policy => policy.RequireClaim("Gender","Female"));
            options.AddPolicy("Under10", policy => policy.RequireAssertion(context =>
                Int32.Parse(context.User.Claims.First(x => x.Type == "Age").Value) < 10));
        });
        
        
    return services;
    }
}