using System.Runtime.InteropServices;
using Common.DTOs;
using DataAccess.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DataAccess.Services;

public class QuizRepositories
{
    private readonly IMongoCollection<Quiz> _quiz;

    public QuizRepositories()
    {
        var hostName = "localhost";
        var port = "27017";
        var databaseName = "QuizDb";
        var client = new MongoClient($"mongodb://{hostName}:{port}");
        var database = client.GetDatabase(databaseName);
        _quiz =
            database.GetCollection<Quiz>("Quizzes", new MongoCollectionSettings() { AssignIdOnInsert = true });
    }

    //ADDERAD
    public static event Action QuizListChanged;
    public void AddQuiz(QuizRecord quizRecord)
    {
        var newQuiz = new Quiz()
        {
            QuizName = quizRecord.QuizName,
            Description = quizRecord.Description,
            Questions = quizRecord.Questions.Select(ObjectId.Parse).ToList()

        };
        _quiz.InsertOne(newQuiz);

        //ADDERAT
        QuizListChanged.Invoke();
    }

    public List<QuizRecord> GetAllQuizzes()
    {
        var filter = Builders<Quiz>.Filter.Empty;

        var allQuizzes = _quiz
            .Find(filter)
            .ToList()
            .Select(qi => new QuizRecord(qi.Id.ToString(), qi.QuizName, qi.Description, qi.Questions.Select(q => q.ToString()).ToList()));

        return allQuizzes.ToList();
    }



    public void DeleteQuiz(string id)
    {
        var filter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(id));
        _quiz.DeleteOne(filter);

        //ADDERAT
        QuizListChanged.Invoke();

    }


    public QuizRecord UpdateQuiz(string id, string newQuizName, string newDescription, List<ObjectId> newQuestionList)
    {
        var filter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(id));

        var update = Builders<Quiz>.Update
            .Set(quiz => quiz.QuizName, newQuizName)
            .Set(quiz => quiz.Description, newDescription)
            .Set(quiz => quiz.Questions, newQuestionList);

        _quiz.UpdateOne(filter, update);

        var updatedQuiz = _quiz.Find(filter).First();

        return new QuizRecord(updatedQuiz.Id.ToString(), updatedQuiz.QuizName, updatedQuiz.Description,
            updatedQuiz.Questions.Select(q => q.ToString()).ToList());
    }






    public Quiz GetQuizById(ObjectId quizId)
    {
        var filter = Builders<Quiz>.Filter.Eq(q => q.Id, quizId);
        return _quiz.Find(filter).FirstOrDefault();
    }

}


