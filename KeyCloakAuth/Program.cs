using KeyCloakAuth.Authorization;
using KeyCloakAuth.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
            o.AddPolicy(nameof(Roles.test), 
                policy =>
                    policy.Requirements
                        .Add(new AuthRequirement(Roles.Admin)));
        });
        builder.Services.AddSingleton<IAuthorizationHandler, AuthRequirementHandler>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
                options.Audience = builder.Configuration["Authentication:Audience"];
                options.TokenValidationParameters = new TokenValidationParameters();

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