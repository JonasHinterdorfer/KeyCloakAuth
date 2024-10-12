using Microsoft.OpenApi.Models;

namespace KeyCloakAuth.Swagger;

public static class SwaggerExtensions
{
   internal static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection service, IConfiguration configuration)
   {
       service.AddSwaggerGen(o =>
           {
               o.SwaggerDoc("v1", new OpenApiInfo { Title = "KeyCloakAuth", Version = "v1" });

               o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));
               o.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
               {
                   Type = SecuritySchemeType.OAuth2,
                   Flows = new OpenApiOAuthFlows
                   {
                       Implicit = new OpenApiOAuthFlow
                       {
                           AuthorizationUrl = new Uri(configuration["Keycloak:AuthorizationUrl"]!),
                           Scopes = new Dictionary<string, string>
                           {
                               {"openid", "openid"},
                               {"profile", "profile"}
                           }
                       }
                   }
               });

               var securityRequirement = new OpenApiSecurityRequirement
               {
                   {
                       new OpenApiSecurityScheme
                       {
                           Reference = new OpenApiReference
                           {
                               Id = "Keycloak",
                               Type = ReferenceType.SecurityScheme
                           },
                           In = ParameterLocation.Header,
                           Name = "Authorization",
                           Scheme = "Bearer"
                       },
                       new List<string>()
                   }
               };

               o.AddSecurityRequirement(securityRequirement);
           }
       );

       return service;
   }
}