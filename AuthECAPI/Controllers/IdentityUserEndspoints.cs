using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthECAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthECAPI.Controllers;

public class UserRegistrationModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
    public string Gender { get; set; }
    public int Age { get; set; }
    public int? LibraryID { get; set; }
}

public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UserDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
}

public static class IdentityUserEndspoints
{
    public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
    {
       app.MapPost("/auth/signup", CreateUser);
       app.MapPost("/auth/signin", SignIn);
        
        return app;
    }

    [AllowAnonymous]
    private static async Task<IResult> CreateUser(UserManager<ApiUser> userManager, 
        [FromBody] UserRegistrationModel userRegistrationModel){
        ApiUser newUser = new ApiUser()
        {
            UserName = userRegistrationModel.Email,
            Email = userRegistrationModel.Email,
            FullName = userRegistrationModel.FullName,
            Gender = userRegistrationModel.Gender,
            DOB = DateOnly.FromDateTime(DateTime.Now.AddYears(-userRegistrationModel.Age)) ,
            LibraryID = userRegistrationModel.LibraryID
        };
        var result = await userManager.CreateAsync(
            newUser, 
            userRegistrationModel.Password);
        await userManager.AddToRoleAsync(newUser,userRegistrationModel.Role);
        if (result.Succeeded)
            return Results.Ok(newUser);
        else
        {
            return Results.BadRequest(newUser);
        }   
    }
    
    [AllowAnonymous]
    private static async Task<IResult> SignIn(UserManager<ApiUser> userManager, 
        [FromBody] LoginModel userLoginModel, IOptions<AppSettings> appSettings){
        var user = await userManager.FindByNameAsync(userLoginModel.Email);
        if (user != null && await userManager.CheckPasswordAsync(user, userLoginModel.Password))
        {
            var roles = await userManager.GetRolesAsync(user); 
            var signInKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(appSettings.Value.JWTSecret));
            ClaimsIdentity claims = new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim("Gender", user.Gender.ToString()),
                new Claim("Age", (DateTime.Now.Year - user.DOB.Year).ToString()),
            });
            if (user.LibraryID != null)
            {
              claims.AddClaim(new Claim("LibraryID", user.LibraryID.ToString()!));  
            }
            var tokenDesctiptor = new SecurityTokenDescriptor
            {
                Subject = claims ,
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDesctiptor);
            var token = tokenHandler.WriteToken(securityToken);
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                UserName = user.UserName
            };
            return Results.Ok(new { token, user = userDto });
        }
        else
        {
            return Results.BadRequest(new {message = "Email or password is incorrect."});
        }
    }
}
