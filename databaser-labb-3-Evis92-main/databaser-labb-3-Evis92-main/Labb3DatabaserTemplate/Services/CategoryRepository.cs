using Common.DTOs;
using DataAccess.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccess.Services;

public class CategoryRepository
{
    private readonly IMongoCollection<Category> _category;

    public CategoryRepository()
    {
        var hostName = "localhost";
        var port = "27017";
        var databaseName = "QuizDb";

        var client = new MongoClient($"mongodb://{hostName}:{port}");

        var database = client.GetDatabase(databaseName);
        
        _category = database.GetCollection<Category>("Categories",
            new MongoCollectionSettings() { AssignIdOnInsert = true });
    }

    public static event Action CategoryListChanged;

    public void AddCategory(CategoryRecord categoryRecord)
    {
        var newCategory = new Category()
        {
            CategoryName = categoryRecord.CategoryName
            
        };
        _category.InsertOne(newCategory);
        CategoryListChanged.Invoke();
    }


    public void DeleteCategory(string id)
    {
        var filter = Builders<Category>.Filter.Eq("Id", ObjectId.Parse(id));
        _category.DeleteOne(filter);
        CategoryListChanged.Invoke();
    }


    public List<CategoryRecord> GetAllCategories()
    {

        var filter = Builders<Category>.Filter.Empty;

        var allCategories = _category
            .Find(filter)
            .ToList()
            .Select(c => new CategoryRecord(c.Id.ToString(), c.CategoryName));

        return allCategories.ToList();
    }


    public bool CategoryExists(string category)
    {
        var filter = Builders<Category>.Filter.Eq(c => c.CategoryName, category);

        var result = _category.Find(filter).Any();
        return result;
    }
}


