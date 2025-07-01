using Carter;
using Mentorly.Profile.EntityModels;
using MongoDB.Driver;

namespace Mentorly.Profile.Endpoints;

public class AddSkillEndpoint:ICarterModule
{
    public class AddSkillRequest
    {
        public string SkillName { get; set; }
        public int ProficiencyLevel { get; set; }
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/AddSkill",async (string userId, AddSkillRequest model, IMongoDatabase db) =>
        {
            var collection = db.GetCollection<ProfileEntity>(ProfileEntity.CollectionName);

            var filter = Builders<ProfileEntity>.Filter.Eq(p => p.UserId,userId);

            var update = Builders<ProfileEntity>.Update.Push(p => p.Skills,new ProfileEntity.Skill()
            {
                Id = Guid.NewGuid(),
                Name = model.SkillName,
                ProficiencyLevel = model.ProficiencyLevel
            });

            var result = await collection.UpdateOneAsync(filter, update);
            
            if(result.IsAcknowledged && result.ModifiedCount > 0)
                return Results.Ok();
            
            return Results.NotFound();
        });
    }
}