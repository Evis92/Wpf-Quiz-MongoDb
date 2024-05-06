using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuizWpf.Models;

public class QuestionModel : INotifyPropertyChanged 
{

    public string Id { get; set; }


    private string _questionTxt;
    public string QuestionText
    {
        get { return _questionTxt; }
        set
        {
            _questionTxt = value;
            OnPropertyChanged();
        }
    }

    private string _category;

    public string Category
    {
        get { return _category; }
        set
        {
            _category = value; 
            OnPropertyChanged();
        }
    }

    private List<string> _answerOptions;

    public List<string> AnswerOptions
    {
        get { return _answerOptions; }
        set
        {
            _answerOptions = value;
            OnPropertyChanged();
        }
    }

    private string _correctAnswer;

    public string CorrectAnswer
    {
        get { return _correctAnswer; }
        set
        {
            _correctAnswer = value; 
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