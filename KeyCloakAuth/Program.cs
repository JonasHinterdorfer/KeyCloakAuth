using KeyCloakAuth.Authorization;
using KeyCloakAuth.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
        builder.Services.AddSwaggerGenWithAuth(builder.Configuration);

        
        builder.Services.AddAuthorization(o =>
        {
            foreach (var role in Enum.GetValues<Roles>())
            {
                o.AddPolicy(role.ToString(), p => p.RequireAuthenticatedUser().RequireRole(role.ToString()));
            }
            
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
                options.Audience = builder.Configuration["Authentication:Audience"];
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = builder.Configuration["Authentication:ValidIssuer"],
                    RoleClaimType = "realm_access.roles",
                };
            });
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // Ensure CORS middleware is added before authentication middleware
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}