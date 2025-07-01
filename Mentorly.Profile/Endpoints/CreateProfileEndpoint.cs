using Carter;
using Mentorly.Profile.EntityModels;
using MongoDB.Driver;

namespace Mentorly.Profile.Endpoints;

public class CreateProfileEndpoint:ICarterModule
{
    public class CreateProfileApiModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; } 
        public string Bio { get; set; }
        public string Timezone { get; set; }

        public List<SkillModel> Skills { get; set; } = [];
        public List<ExperienceModel> Experiences { get; set; } = [];
        public List<SocialLink> SocialLinks { get; set; } = [];
        public class SkillModel
        {
            public string Name { get; set; }
            public int ProficiencyLevel { get; set; }
        }
    
        public class ExperienceModel
        {
        
            public string Title { get; set; }
            public string Company { get; set; }
            public string Description { get; set; }
            public DateTime From { get; set; }
            public DateTime? To { get; set; }
        }
    
        public class SocialLink
        {
            public string Platform { get; set; }
            public string Url { get; set; }
        }
        
        public ProfileEntity ToEntity(string userId)
        {
            return new ProfileEntity()
            {
                UserId = userId,
                Bio = this.Bio,
                Email = this.Email,
                Experiences = this.Experiences.Select(e => new ProfileEntity.Experience
                {
                    Id = Guid.NewGuid(),
                    Title = e.Title,
                    Company = e.Company,
                    Description = e.Description,
                    From = e.From,
                    To = e.To
                }).ToList(),

                FullName = this.FullName,
                Skills = this.Skills.Select(s => new ProfileEntity.Skill
                {
                    Id = Guid.NewGuid(),
                    Name = s.Name,
                    ProficiencyLevel = s.ProficiencyLevel
                }).ToList(),
                SocialLinks = this.SocialLinks.Select(s => new ProfileEntity.SocialLink
                {
                    Id = Guid.NewGuid(),
                    Platform = s.Platform,
                    Url = s.Url
                }).ToList(),

                Timezone = this.Timezone
            };
        }
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/profile", async (CreateProfileApiModel apiModel, IMongoDatabase db) =>
        {
            var collections = db.GetCollection<ProfileEntity>(ProfileEntity.CollectionName);

            var existUserProfile = Builders<ProfileEntity>.Filter.Eq(c => c.UserId, apiModel.UserId);

            if (await collections.Find(existUserProfile).AnyAsync())
            {
                return Results.Problem("Profile AlreadyExist", statusCode: StatusCodes.Status409Conflict);
            }
            var entity = apiModel.ToEntity(apiModel.UserId);
            await collections.InsertOneAsync(entity);


            return Results.Created($"/Profiles/{entity.Id}", entity);
        });
    }
}