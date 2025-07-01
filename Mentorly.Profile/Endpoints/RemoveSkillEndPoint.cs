using Carter;
using Mentorly.Profile.EntityModels;
using MongoDB.Driver;

namespace Mentorly.Profile.Endpoints;

public class RemoveSkillEndpoint:ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/Profile/RemoveSkillRoute/{userId}/{skillId}", async (string userId, Guid skillId, IMongoDatabase db) =>
        {
            var collection = db.GetCollection<ProfileEntity>(ProfileEntity.CollectionName);
            
            var filter = Builders<ProfileEntity>.Filter.Eq(p => p.UserId, userId);

            var update = Builders<ProfileEntity>.Update
                .PullFilter(c => c.Skills,
                    s => s.Id == skillId);
            
            await collection.UpdateOneAsync(filter, update);

            return Results.Ok();
        });
    }
}