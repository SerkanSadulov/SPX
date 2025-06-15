using API.Services;
using DAL.DAOs;
using DAL.Factories;
using DAL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var jwtSettings = builder.Configuration.GetSection("JwtSettings");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
                };
            });

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid JWT token",
                };

                c.AddSecurityDefinition("Bearer", securityScheme);
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                var securityRequirement = new OpenApiSecurityRequirement
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
            new string[] {}
        }
    };

                c.AddSecurityRequirement(securityRequirement);
            });
            builder.Services.AddAuthorization();

            // CORS Policy
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
                          .SetIsOriginAllowed(_ => true); 
                });
            });

            builder.Services.AddTransient<IFactory<SqlConnection>, DatabaseFactory>();
            builder.Services.AddTransient<IFactory<string>, ServicesFactory>();

            // Register dependency injection for DAOs
            builder.Services.AddTransient<IUserDAO, UserDAO>();
            builder.Services.AddTransient<IMessagesDAO, MessagesDAO>();
            builder.Services.AddTransient<IRatingsDAO, RatingsDAO>();
            builder.Services.AddTransient<IOngoingOrderDAO, OngoingOrderDAO>();
            builder.Services.AddTransient<ICategoriesDAO, CategoriesDAO>();
            builder.Services.AddTransient<IServicesDAO, ServicesDAO>();
            builder.Services.AddTransient<IFavoritesDAO, FavoritesDAO>();

            // Add SignalR
            builder.Services.AddSignalR();

            // Add OpenAI service
            builder.Services.AddScoped<OpenAIService>();

            // Add HttpClient registration
            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI();

            // Redirect HTTP to HTTPS
            app.UseHttpsRedirection();

            // Routing
            app.UseRouting();

            // Enable CORS
            app.UseCors();

            // Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Map the controllers and endpoints

            app.MapControllers();


            // Run the application
            app.Run();
        }
    }
}