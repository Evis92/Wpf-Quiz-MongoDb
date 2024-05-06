using MongoDB.Bson;

namespace DataAccess.Entities;

public class Quiz
{
    public ObjectId Id { get; set; }

    public string QuizName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<ObjectId> Questions { get; set; } = new List<ObjectId>();
}

