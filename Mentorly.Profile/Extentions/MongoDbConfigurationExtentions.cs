using Mentorly.Profile.MongoDb.Configuration;
using MongoDB.Driver;

namespace Mentorly.Profile.Extentions;

public static class MongoDbConfigurationExtentions
{
    public static WebApplicationBuilder ConfigureMongoDb(this WebApplicationBuilder builder)
    {
        var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoDb"));
        var mongoDatabase = mongoClient.GetDatabase("Mentorly-ProfileService");
        builder.Services.AddSingleton(mongoDatabase);
        return builder;
    }

    public static WebApplicationBuilder ConfigureMongoDbEntities(this WebApplicationBuilder builder)
    {
        var configurationTypeServices = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IMongoDbEntityConfiguration).IsAssignableFrom(type) && type is{IsInterface:false,IsAbstract:false} )
            .Select(type=>new ServiceDescriptor(typeof(IMongoDbEntityConfiguration),type, ServiceLifetime.Singleton));
        foreach (var configurationTypeService in configurationTypeServices)
        {
            builder.Services.Add(configurationTypeService);
        }
        return builder;
    }

    public static async Task UseMongoDbEntitiesAsync(this WebApplication app)
    {
        var mongoDatabase = app.Services.GetRequiredService<IMongoDatabase>();
        var configurations = app.Services.GetServices<IMongoDbEntityConfiguration>();
        foreach (var configuration in configurations)
        {
            await configuration.ConfigureAsync(mongoDatabase);
        }
    }
}