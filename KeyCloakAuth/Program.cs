using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Startup.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace KeyCloakAuth;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        // Add CORS services
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        // Microsoft.AspNetCore.Authentication.OpenIdConnect
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options => { options.LoginPath = "/Account/Login"; })
            .AddOpenIdConnect(options =>
            {
                options.Authority = "https://auth.registerplatform.ionas999.at/realms/master";
                options.ClientId = "RegisterPlatformBackend";
                options.ClientSecret = "fwHz5M4epcGWFJQOzFrRWEc9m2WsMgHA"; // get this from a secure place
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.Scope.Add("openid");
                options.CallbackPath = "/login-callback"; // Update callback path
                options.SignedOutCallbackPath = "/logout-callback"; // Update signout callback path
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "preferred_username",
                    RoleClaimType = "roles"
                };
            });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");

        // Ensure CORS middleware is added before authentication middleware
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}