using Common.DTOs;
using DataAccess.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace DataAccess.Services;

public class QuestionRepositories
{
    private readonly IMongoCollection<Question> _questions;

    public QuestionRepositories()
    {
        var hostName = "localhost";
        var port = "27017";
        var databaseName = "QuizDb";
        var client = new MongoClient($"mongodb://{hostName}:{port}");
        var database = client.GetDatabase(databaseName);
        _questions =
            database.GetCollection<Question>("Questions", new MongoCollectionSettings() { AssignIdOnInsert = true });
    }

    public static event Action QuestionListChanged;


    public List<QuestionRecord> GetAllQuestions()
    {
        var filter = Builders<Question>.Filter.Empty;

        var allQuestions = _questions
            .Find(filter)
            .ToList()
            .Select(q => new QuestionRecord(q.Id.ToString(), q.QuestionTxt, q.Category, q.AnswerOptions, q.CorrectAnswer));
            
        return allQuestions.ToList();
        
    }



    public void AddQuestion(QuestionRecord questionRecord)
    {
        var newQuestion = new Question()
        {
            QuestionTxt = questionRecord.QuestionTxt,
            Category = questionRecord.Category,
            AnswerOptions = questionRecord.AnswerOptions,
            CorrectAnswer = questionRecord.CorrectAnswer
        };

        _questions.InsertOne(newQuestion);
        QuestionListChanged.Invoke();
    }



    public void DeleteQuestion(string id)
    {
        var filter = Builders<Question>.Filter.Eq("Id", ObjectId.Parse(id));
        _questions.DeleteOne(filter);
        QuestionListChanged.Invoke();
    }



    public QuestionRecord UpdateQuestion(string id, string newQuestion, string newCategory, List<string> newAnswerOptions, string newCorrectAnswer)
    {
        var filter = Builders<Question>.Filter.Eq("_id", ObjectId.Parse(id));
        
        
        var update = Builders<Question>.Update
            .Set(question => question.QuestionTxt, newQuestion)
            .Set(question => question.Category, newCategory)
            .Set(question => question.AnswerOptions, newAnswerOptions)
            .Set(question => question.CorrectAnswer, newCorrectAnswer);

        _questions.UpdateOne(filter, update);

        var updatedQuestion = _questions.Find(filter).FirstOrDefault();

        return new QuestionRecord(updatedQuestion.Id.ToString(), updatedQuestion.QuestionTxt, updatedQuestion.Category,
            updatedQuestion.AnswerOptions, updatedQuestion.CorrectAnswer);
    }


    public QuestionRecord GetQuestionById(ObjectId questionId)
    {
        var filter = Builders<Question>.Filter.Eq(q => q.Id, questionId);
        var foundQuestion = _questions.Find(filter).FirstOrDefault();

        if (foundQuestion != null)
        {
            return new QuestionRecord(
                foundQuestion.Id.ToString(),
                foundQuestion.QuestionTxt,
                foundQuestion.Category,
                foundQuestion.AnswerOptions,
                foundQuestion.CorrectAnswer
            );
        }

        return null;
    }
}
