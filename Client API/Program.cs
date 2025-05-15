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

namespace Client_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<E_LearningDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

            builder.Services.AddIdentity<BaseUser, IdentityRole>()
                .AddEntityFrameworkStores<E_LearningDbContext>()
                .AddDefaultTokenProviders();

            //builder.Services.AddIdentity<BaseUser>().

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            JwtSettings jwtOptions = builder.Configuration.GetSection("JWT").Get<JwtSettings>() ?? throw new Exception("Error in JWT Settings");

            builder.Services.AddSignalR();
            builder.Services.AddSingleton<JwtSettings>(jwtOptions);

            builder.Services.AddScoped<IAuth, AuthService>();
            builder.Services.AddScoped<ICourse, CourseService>();
            builder.Services.AddScoped<IGoogleMeetAuthService, GoogleMeetAuthService>();
            builder.Services.AddScoped<IMeetService, MeetService>();
            builder.Services.AddScoped<IZoomAuthService, ZoomAuthService>();
            builder.Services.AddScoped<IZoomService, ZoomService>();
            builder.Services.AddScoped<IStripeService, StripeService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IReviewsService, InstructorReviewsService>();
            builder.Services.AddScoped<IJWT, JWTService>();
            builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
            builder.Services.AddScoped<IBlogService, BlogService>();
            builder.Services.AddScoped<IContactForm, ContactFormService>();

            builder.Services.AddScoped<ImagesUploadsService>();

            //builder.Services.AddScoped<typeof(IGenericRepo<>), typeof(GenericRepo<>));

            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));

            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddHttpClient();


            //builder.Services.AddCors(options =>
            //{
            //options.AddPolicy("AllowAll", policy =>
            //    policy.AllowAnyOrigin()
            //    .AllowAnyHeader()
            //    .AllowAnyMethod()
            //    .AllowCredentials();
            //}));

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
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("AllowAll");

            app.MapControllers();

            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
        //public void ConfigerWhatsAppService(IServiceCollection services)
        //{
        //    services.AddScoped<IWhatsAppService, WhatsAppService>();
        //    //services.AddHostedService<whatsa>
        //}
    }
}
