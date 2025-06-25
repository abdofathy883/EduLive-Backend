using Core.Interfaces;
using Core.Models;
using Core.Settings;
using Infrastructure.Configrations;
using Infrastructure.Data;
using Infrastructure.Repos;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using E_LearningDbContextAlias = Infrastructure.Data.E_LearningDbContext;

namespace eLearning_Admin
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<E_LearningDbContextAlias>(options =>
                options.UseSqlServer(connectionString));


            builder.Services.AddIdentity<BaseUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<E_LearningDbContextAlias>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("MailSettings"));

            builder.Services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/", "RequireAuth");
                options.Conventions.AllowAnonymousToPage("/Account/Login");
                options.Conventions.AllowAnonymousToPage("/Account/AccessDenied");
            });

            builder.Services.AddScoped<ICourse, CourseService>();
            builder.Services.AddScoped<IBlogService, BlogService>();
            builder.Services.AddScoped<IAuth, AuthService>();

            builder.Services.AddScoped<MediaUploadsService>();

            //builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddTransient<IEmailSender, EmailService>();
            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));

            JwtSettings jwtOptions = builder.Configuration.GetSection("JWT").Get<JwtSettings>() ?? throw new Exception("Error in JWT Settings");
            builder.Services.AddSingleton<JwtSettings>(jwtOptions);
            builder.Services.AddScoped<IJWT, JWTService>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAuth", policy => policy.RequireAuthenticatedUser());
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.LogoutPath = "/Identity/Account/Logout";
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
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

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Use Always in production
                options.Cookie.SameSite = SameSiteMode.Lax;
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
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            await SeedTestUser(app);

            app.Run();
        }

        private static async Task SeedTestUser(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<BaseUser>>();

            const string testEmail = "abdofathy99@outlook.com";
            const string testPassword = "Aa123#";

            var existingUser = await userManager.FindByEmailAsync(testEmail);
            if (existingUser == null)
            {
                var testUser = new BaseUser
                {
                    FirstName = "Abdo",
                    LastName = "Admin",
                    UserName = testEmail,
                    Email = testEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(testUser, testPassword);
                if (result.Succeeded)
                {
                    Console.WriteLine($"Test user created: {testEmail} / {testPassword}");
                }
                else
                {
                    Console.WriteLine("Failed to create test user:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                    }
                }
            }
        }
    }  
}
