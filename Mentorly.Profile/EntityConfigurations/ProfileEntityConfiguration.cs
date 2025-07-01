using Mentorly.Profile.EntityModels;
using Mentorly.Profile.MongoDb.Configuration;
using MongoDB.Driver;

namespace Mentorly.Profile.EntityConfigurations;

public class ProfileEntityConfiguration:IMongoDbEntityConfiguration
{
    public async Task ConfigureAsync(IMongoDatabase database)
    {
        var indexModel=new CreateIndexModel<ProfileEntity>(
            Builders<ProfileEntity>.IndexKeys.Text(p => p.UserId),
            new CreateIndexOptions { Unique = true }
        );
        
        var collection = database.GetCollection<ProfileEntity>(ProfileEntity.CollectionName);
        await collection.Indexes.CreateOneAsync(indexModel);
    }
}