﻿using Confluent.Kafka;
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

            // Add event consumer related dependencies

            service.AddSingleton(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var config = new ConsumerConfig
                {
                    BootstrapServers = configuration["KafkaSettings:BootstrapServer"],
                    GroupId = configuration["KafkaSettings:ConsumerGroupId"],
                    AutoOffsetReset = Enum.Parse<AutoOffsetReset>(configuration["KafkaSettings:AutoOffsetReset"]),
                    EnableAutoCommit = bool.Parse(configuration["KafkaSettings:EnableAutoCommit"]),
                    SecurityProtocol = Enum.Parse<SecurityProtocol>(configuration["KafkaSettings:SecurityProtocol"]),
                    SaslMechanism = Enum.Parse<SaslMechanism>(configuration["KafkaSettings:SaslMechanism"]),
                    SaslUsername = configuration["KafkaSettings:SaslUsername"],
                    SaslPassword = configuration["KafkaSettings:SaslPassword"]
                };
                return config;
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