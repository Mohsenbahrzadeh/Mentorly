using Carter;
using Mentorly.Profile.EntityModels;
using MongoDB.Driver;

namespace Mentorly.Profile.Endpoints;

public class ProfileSummaryEndpoint:ICarterModule
{
    public class ProfileSummaryResult
    {
        public string? ProfileId { get; set; }
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? UserId { get; set; }
        public string[] Skills { get; set; }
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/ProfileSummary/{userId}", async (IMongoDatabase db,string userId) =>
        {
            var collection=db.GetCollection<ProfileEntity>(ProfileEntity.CollectionName);
            
            var filter = Builders<ProfileEntity>.Filter.Eq(p => p.UserId, userId);
            
            var result = await collection.Find(filter)
                .Project(p=>new ProfileSummaryResult()
                {
                    FullName = p.FullName,
                    Bio = p.Bio,
                    ProfileId = p.Id.ToString(),
                    UserId = p.UserId,
                    Skills = p.Skills.Select(s=>s.Name).ToArray()
                }).FirstOrDefaultAsync();
            
            return result != null ? Results.Ok(result) : Results.NotFound();
        });
    }
}