using MongoDB.Driver;

namespace Mentorly.Profile.MongoDb.Configuration;

public interface IMongoDbEntityConfiguration
{
    Task ConfigureAsync(IMongoDatabase database);
}