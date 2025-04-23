using System.Text;
using Infrastructure.Configurations;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddDbContext<MediCallContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).UseLazyLoadingProxies());

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            JwtSettings JwtOption = builder.Configuration.GetSection("JWT").Get<JwtSettings>() ?? throw new Exception("Error in JWT in app setting");

            builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, option =>
            {
                option.SaveToken = true;
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = JwtOption.Issuer,
                    ValidAudience = JwtOption.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOption.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}