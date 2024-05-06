using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common.DTOs;
using DataAccess.Services;
using QuizWpf.Models;

namespace QuizWpf.Views
{
    /// <summary>
    /// Interaction logic for CategoryView.xaml
    /// </summary>
    public partial class CategoryView : UserControl
    {
        private readonly CategoryRepository _catRepo;

        public ObservableCollection<CategoryModel> CategoryList { get; set; } = new();

        public CategoryModel SelectedCategory { get; set; } = new();

        public string CategoryName { get; set; }

        ////---------------------------------------------------------EVENT


        //public event EventHandler CategoryAdded;

        //protected virtual void OnCategoryAdded()
        //{
        //    CategoryAdded?.Invoke(this, EventArgs.Empty);
        //}


        //-----------------------------------------------------------------------------

        public CategoryView()
        {
            InitializeComponent();

            DataContext = this;

            _catRepo = new CategoryRepository();


            //-------------------------------Fyller ListVyn med alla Kategorier---------------------------------------

            var allCategories = _catRepo.GetAllCategories();
            foreach (var category in allCategories)
            {
                CategoryList.Add(new CategoryModel(){CategoryName = category.CategoryName, Id = category.Id});
            }
            //---------------------------------------------------------------------------------------------------------
        }


        private void AddBtn_OnClick(object sender, RoutedEventArgs e)
        {
            
            string categoryName = CategoryTxtBox.Text;

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                MessageBox.Show("Enter a Category in the textbox");
                return;
            }

            if (_catRepo.CategoryExists(categoryName) == true)
            {
                MessageBox.Show("Category already exists");
                return;
            }

            CategoryModel categoryModel = new CategoryModel();
            categoryModel.CategoryName = CategoryTxtBox.Text;

            var newCategory = new CategoryRecord("", CategoryTxtBox.Text);
            
            _catRepo.AddCategory(newCategory);
            

            //-------------------Adderar kategorin till ListVyn------------------------------------------------
            //CategoryList.Add(categoryModel);

            CategoryList.Clear();
            var allCategories = _catRepo.GetAllCategories();
            foreach (var category in allCategories)
            {
                CategoryList.Add(new CategoryModel() { CategoryName = category.CategoryName, Id = category.Id });
            }



            MessageBox.Show($"You have added {CategoryTxtBox.Text}");
            CategoryTxtBox.Clear();

        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var categoryToDelete = AllCategoriesListView.SelectedItem as CategoryModel;

            if (categoryToDelete == null)
            {
                MessageBox.Show($"You have to select a category you want to remove");
                return;
            }

            MessageBox.Show($"You have removed {categoryToDelete.CategoryName}");
            _catRepo.DeleteCategory(categoryToDelete.Id);

            CategoryList.Remove(categoryToDelete);
        }

        private void AllCategoriesListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllCategoriesListView.SelectedItem != null)
            {
                var selectedCategory = AllCategoriesListView.SelectedItem as CategoryModel;
            }
            
        }
    }
}
