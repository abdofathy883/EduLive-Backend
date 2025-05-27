using Core.Interfaces;
using Core.Models;
using Infrastructure.Configrations;
using Infrastructure.Data;
using Infrastructure.Repos;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using E_LearningDbContextAlias = Infrastructure.Data.E_LearningDbContext;

namespace eLearning_Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<E_LearningDbContextAlias>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<BaseUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<E_LearningDbContextAlias>()
                .AddDefaultTokenProviders();

            builder.Services.AddRazorPages(options =>
            {
                //options.Conventions.AuthorizeFolder("/", "RequireAuth");
                options.Conventions.AllowAnonymousToPage("/Account/Login");
                options.Conventions.AllowAnonymousToPage("/Account/AccessDenied");
            });


            //builder.Services.AddScoped<>
            builder.Services.AddScoped<ICourse, CourseService>();
            builder.Services.AddScoped<IBlogService, BlogService>();
            builder.Services.AddScoped<IAuth, AuthService>();

            builder.Services.AddScoped<ImagesUploadsService>();
            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));

            JwtSettings jwtOptions = builder.Configuration.GetSection("JWT").Get<JwtSettings>() ?? throw new Exception("Error in JWT Settings");
            builder.Services.AddSingleton<JwtSettings>(jwtOptions);
            builder.Services.AddScoped<IJWT, JWTService>();

            //builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options =>
            //    {
            //        options.LoginPath = "/Account/Login";
            //        options.AccessDeniedPath = "/Account/AccessDenied";
            //        options.ExpireTimeSpan = TimeSpan.FromHours(24);
            //    });

            //builder.Services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("RequireAuth", policy => policy.RequireAuthenticatedUser());
            //});


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
