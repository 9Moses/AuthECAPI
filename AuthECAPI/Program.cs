using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthECAPI.Controllers;
using AuthECAPI.Extension;
using AuthECAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Fill in the JWT token"
        });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,    
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

//Service for Identity core
builder.Services.InjectDbContext(builder.Configuration)
    .AddIdentityHandlersAndStore()
    .ConfigureIdentityOptions()
    .AddIdentityAuth(builder.Configuration);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

// app
//     .MapGroup("/api")
//     .MapIdentityApi<ApiUser>();
app.MapGroup("/api")
    .MapIdentityUserEndpoints()
    .MapAccountEndpoints()
    .MapAuthorizationDemoEndpoint();


app.Run();

