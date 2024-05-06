
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAccess.Entities;

public class Question
{
    public ObjectId Id { get; set; }


    public string QuestionTxt { get; set; } = string.Empty;


    public string Category { get; set; } = string.Empty;

    public List<string> AnswerOptions { get; set; } = new List<string>();

    public string CorrectAnswer { get; set; } = string.Empty;
}