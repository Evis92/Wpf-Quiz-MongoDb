using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuizWpf.Models;

public class CategoryModel : INotifyPropertyChanged
{

    public string Id { get; set; }



    private string _categoryName;
    public string CategoryName
    {
        get { return _categoryName; }
        set
        {
            _categoryName = value;
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