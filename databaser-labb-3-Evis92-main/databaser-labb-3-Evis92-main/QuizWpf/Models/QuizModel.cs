using System.ComponentModel;
using System.Runtime.CompilerServices;
using MongoDB.Bson;

namespace QuizWpf.Models;

public class QuizModel : INotifyPropertyChanged
{
    private string _id;
    public string Id
    {
        get { return _id; }
        set
        {
            _id = value;
            OnPropertyChanged();
        }
    }


    private string _quizName;

    public string QuizName
    {
        get { return _quizName; }
        set
        {
            _quizName = value;
            OnPropertyChanged();
        }
    }


    private string _description;

    public string Description
    {
        get { return _description; }
        set
        {
            _description = value;
            OnPropertyChanged();
        }
    }


    private List<ObjectId> _questions;

    public List<ObjectId> Questions
    {
        get { return _questions; }
        set
        {
            _questions = value; 
            OnPropertyChanged();
        }
    }




    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}



