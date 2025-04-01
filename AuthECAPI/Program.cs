using AuthECAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Service for Identity core
builder.Services
    .AddIdentityApiEndpoints<ApiUser>()
    .AddEntityFrameworkStores<AppDBContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
});

//Add DBContext
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DevConnection")));

// builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//     .AddEntityFrameworkStores<AppDBContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app
    .MapGroup("/api")
    .MapIdentityApi<ApiUser>();

app.MapPost("/api/signup", async (UserManager<ApiUser> userManager, 
        [FromBody] UserRegistrationModel userRegistrationModel) =>
{
    ApiUser newUser = new ApiUser()
    {
        UserName = userRegistrationModel.Email,
        Email = userRegistrationModel.Email,
        FullName = userRegistrationModel.FullName,
    };
   var result = await userManager.CreateAsync(
       newUser, 
       userRegistrationModel.Password);

   if (result.Succeeded)
       return Results.Ok(newUser);
   else
   {
       return Results.BadRequest(newUser);
   }
});

app.Run();

public class UserRegistrationModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
}