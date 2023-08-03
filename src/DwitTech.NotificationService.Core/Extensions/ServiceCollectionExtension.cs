using Confluent.Kafka;
using Confluent.Kafka.DependencyInjection;
using DwitTech.NotificationService.Core.Dtos;
using DwitTech.NotificationService.Core.Interfaces;
using DwitTech.NotificationService.Core.Services;
using DwitTech.NotificationService.Data.Context;
using DwitTech.NotificationService.Data.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionsExtension
    {
        public static IServiceCollection AddDatabaseService(this IServiceCollection service, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("NotificationDbContext");
            connectionString = connectionString.Replace("{DBHost}", configuration["DB_HOSTNAME"]);
            connectionString = connectionString.Replace("{DBName}", configuration["DB_NAME"]);
            connectionString = connectionString.Replace("{DBUser}", configuration["DB_USERNAME"]);
            connectionString = connectionString.Replace("{DBPassword}", configuration["DB_PASSWORD"]);

            service.AddDbContext<NotificationDbContext>(opt =>
            {
                opt.UseNpgsql(connectionString, c => c.CommandTimeout(120));
#if DEBUG
                opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                opt.EnableSensitiveDataLogging();
#endif
            },
            contextLifetime: ServiceLifetime.Scoped,
            optionsLifetime: ServiceLifetime.Scoped);

            return service;
        }

        public static IServiceCollection AddServices(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            service.AddScoped<IEmailRepo, EmailRepo>();
            service.AddScoped<IEmailService, EmailService>();
            service.AddSingleton<CancellationTokenSource>();
            // Add event consumer related dependencies
            service.AddScoped<IEmailConsumerService, EmailConsumerService>();
            service.AddSingleton<IConsumer<Ignore, string>>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var config = new ConsumerConfig
                {
                    BootstrapServers = configuration["MESSAGE_BROKER_BOOTSTRAP_SERVERS"],
                    GroupId = configuration["KAFKA_CONSUMER_GROUP_ID"],
                    AutoOffsetReset = Enum.Parse<AutoOffsetReset>(configuration["KAFKA_AUTO_OFFSET_RESET"]),
                    EnableAutoCommit = bool.Parse(configuration["KAFKA_ENABLE_AUTO_COMMIT"]),
                    SecurityProtocol = Enum.Parse<SecurityProtocol>(configuration["KAFKA_SECURITY_PROTOCOL"]),

                };

                switch(config.SecurityProtocol)
                {
                    case null:
                    case SecurityProtocol.Plaintext:
                        break;
                    case SecurityProtocol.Ssl:
                        break;
                    
                    case SecurityProtocol.SaslSsl:
                        config.SaslMechanism = Enum.Parse<SaslMechanism>(configuration["KAFKA_SASL_MECHANISM"]);
                        config.SaslUsername = configuration["KAFKA_SASL_USERNAME"];
                        config.SaslPassword = configuration["KAFKA_SASL_PASSWORD"];
                        break;

                    case SecurityProtocol.SaslPlaintext:
                        throw new NotImplementedException($"Security Protocol {config.SecurityProtocol} is not implemented");
                       
                }
                return new ConsumerBuilder<Ignore, string>(config).Build();
            });

            service.AddHostedService<EmailEventConsumer>();

            return service;
        }


        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["JWT:Authority"];
                options.Audience = configuration["JWT:Audience"];

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidAudiences = new List<string> { configuration["JWT:Audience"] },
                };
            });
        }
    }
}