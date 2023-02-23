using DwitTech.NotificationService.Core.Interfaces;
using DwitTech.NotificationService.Core.Services;
using DwitTech.NotificationService.Data.Context;
using DwitTech.NotificationService.Data.Repository;
using NLog;
using NLog.Fluent;
using NLog.Web;
using System.Text.Json.Serialization;

namespace DwitTech.NotificationService.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();    
               
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    // To preserve the default behavior, capture the original delegate to call later.
                    var builtInFactory = options.InvalidModelStateResponseFactory;

                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                                            .GetRequiredService<ILogger<Program>>();
                        return builtInFactory(context);
                    };
                })
                .AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
                
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddDatabaseService(builder.Configuration);

            //builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGenNewtonsoftSupport();
            builder.Services.AddHealthChecks();
            builder.Services.AddScoped<IEmailRepo, EmailRepo>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddServices(builder.Configuration);
            builder.Host.UseNLog();
            
            // Add service and create Policy with options
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            builder.Services.ConfigureAuthentication(builder.Configuration);

            builder.Services.AddAuthorization();

            var app = builder.Build();

            IConfiguration configuration = app.Configuration;
            IWebHostEnvironment environment = app.Environment;

            // Configure the HTTP request pipeline.
            app.MapHealthChecks("/health");

            app.UseSwagger();
            app.UseSwaggerUI();

           

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.SetupMigrations(app.Services, app.Configuration);

            app.Run();
        }
    }
}