using System.Text;
using System.Threading.Tasks;
using Core.Interface;
using Core.Models;
using Infrastructure.Configurations;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<MediCallContext>()
                .AddDefaultTokenProviders();


            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.WriteIndented = true;
            });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();



            JwtSettings JwtOption = builder.Configuration.GetSection("JWT").Get<JwtSettings>() ?? throw new Exception("Error in JWT settings");
            MailSettings mailSettings = builder.Configuration.GetSection("MailSettings").Get<MailSettings>() ?? throw new Exception("Error in Mail settings");

            builder.Services.AddSignalR();
            builder.Services.AddSingleton<JwtSettings>(JwtOption);
            builder.Services.AddSingleton<MailSettings>(mailSettings);

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IVisitService, VisitService>();
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<IGetDataService, GetDataService>();
            builder.Services.AddScoped<IPatientIllnessesService, PatientIllnessesService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<INotificationService,NotificationService>();
            builder.Services.AddHttpClient();



            builder.Services.AddScoped<IMailingService, MailingService>();
            builder.Services.AddScoped<IUploadFileService, UploadFileService>();
            builder.Services.AddScoped<INurseCertificateService, NurseCertificateService>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, option =>
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


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("AllowAll");

            app.MapControllers();

            app.MapHub<VisitHub>("/VisitHub");
            app.MapHub<NotificationHub>("/hub/notifications");

            app.Run();
        }
    }
}