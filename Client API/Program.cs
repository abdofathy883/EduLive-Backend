using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Infrastructure.Configrations;
using Core.Interfaces;
using Infrastructure.Services;
using Infrastructure.SignalR;
using Stripe;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Repos;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Core.Settings;
using Infrastructure.Background;
using Serilog;

namespace Client_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // DbContext Registration
            builder.Services.AddDbContext<E_LearningDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

            builder.Services.AddIdentity<BaseUser, IdentityRole>()
                .AddEntityFrameworkStores<E_LearningDbContext>()
                .AddDefaultTokenProviders();


            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "E-Learning API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                    } 
                });
            });

            JwtSettings jwtOptions = builder.Configuration.GetSection("JWT").Get<JwtSettings>() ?? throw new Exception("Error in JWT Settings");

            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];


            builder.Services.AddSignalR();
            builder.Services.AddSingleton<JwtSettings>(jwtOptions);

            //Settings
            builder.Services.Configure<ZoomSettings>(builder.Configuration.GetSection("ZoomSettings"));
            builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
            // --------------------------
            // Auth Registration
            builder.Services.AddScoped<IAuth, AuthService>();
            builder.Services.AddScoped<ICourse, CourseService>();
            //Email Registration
            builder.Services.AddScoped<IEmailService, EmailService>();
            //Google Meet Registration
            builder.Services.AddHttpClient<IGoogleMeetAuthService, GoogleMeetAuthService>();
            builder.Services.AddScoped<IGoogleMeetAuthService, GoogleMeetAuthService>();
            builder.Services.AddScoped<IGoogleMeetService, GoogleMeetService>();
            //Zoom Registration
            builder.Services.AddScoped<IZoomAuthService, ZoomAuthService>();
            builder.Services.AddHttpClient<IZoomAuthService, ZoomAuthService>();
            builder.Services.AddScoped<IZoomService, ZoomService>();
            // Payment and Transactions
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<ITransactionsService, TransactionService>();
            // Reviews and Ratings
            builder.Services.AddScoped<ICourseReviews, CourseReviewsService>();
            builder.Services.AddScoped<IReviewsService, InstructorReviewsService>();
            builder.Services.AddScoped<ILessonReport, LessonReportService>();
            builder.Services.AddScoped<IJWT, JWTService>();

            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IBlogService, BlogService>();
            builder.Services.AddScoped<IContactForm, ContactFormService>();

            builder.Services.AddScoped<ICertificateGenerationService, CertificateGenerationService>();

            builder.Services.AddScoped<MediaUploadsService>();

            builder.Services.AddHostedService<WhatsAppReminderBackgroundService>();

            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));

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

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddHttpClient();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowProductionDomain", policy =>
                {
                    policy.WithOrigins("https://tahfezquran.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Learning API v1"));
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowProductionDomain");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}
