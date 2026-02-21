using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Services;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure;
using NotificationService.HealthChecks;
using NotificationService.Persistence.Repositories;
using NotificationService.Templates.Services;
using OpenTelemetry.Trace;

namespace NotificationService.Configuration;

public static class ServiceRegistration
{
    public static IServiceCollection AddNotificationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CosmosOptions>(configuration.GetSection("Cosmos"));
        services.Configure<TwilioOptions>(configuration.GetSection("Twilio"));
        services.Configure<AcsEmailOptions>(configuration.GetSection("AcsEmail"));

        services.AddSingleton(sp =>
        {
            var connectionString = configuration.GetConnectionString("CosmosDb")
                ?? configuration["Cosmos:ConnectionString"];
            return new CosmosClient(connectionString);
        });

        services.AddSingleton<INotificationRepository, CosmosNotificationRepository>();
        services.AddSingleton<ITemplateRenderer, EmbeddedTemplateRenderer>();
        services.AddSingleton<ISmsSender, SmsSender>();
        services.AddSingleton<IEmailSender, EmailSender>();

        services.AddScoped<IIngestNotificationService, IngestNotificationService>();
        services.AddScoped<IDispatchNotificationService, DispatchNotificationService>();
        services.AddScoped<ISendNotificationService, SendNotificationService>();

        services.AddHealthChecks()
            .AddCosmosDb(configuration.GetConnectionString("CosmosDb")!)
            .AddAzureServiceBusQueue(configuration["ServiceBusConnection"]!, "notificationqueue");

        services.AddOptions<TwilioOptions>()
            .BindConfiguration("Twilio")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOpenTelemetry()
            .WithTracing(builder => builder
                .AddSource("NotificationService")
                .AddHttpClientInstrumentation());

        return services;
    }
}
