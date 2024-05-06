using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using DataAccess.Entities;
using DataAccess.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using QuizWpf.Models;

namespace QuizWpf.Views
{
    /// <summary>
    /// Interaction logic for QuestionView.xaml
    /// </summary>
    public partial class QuestionView : UserControl
    {
        private readonly QuestionRepositories _questionRepo;
        private readonly CategoryRepository _categoryRepo;

        public ObservableCollection<QuestionModel> QuestionList { get; set; } = new();

        public ObservableCollection<CategoryModel> CategoryList { get; set; } = new();

        public QuestionModel SelectedQuestion { get; set; } = new();

        public string QuestionTxt { get; set; }

        public string Category { get; set; }

        public List<string> AnswerOptionList { get; set; }

        public string CorrectAnswer { get; set; }




        public QuestionView()
        {
            InitializeComponent();

            CategoryRepository.CategoryListChanged += CategoryRepository_CategoryListChanged;


            DataContext = this;

            _questionRepo = new QuestionRepositories();

            _categoryRepo = new CategoryRepository();

           
            //------------------------------------Adderar alla frågor till ListVyn--------------------------------------------------

            var allQuestions = _questionRepo.GetAllQuestions();
            
            foreach (var question in allQuestions)
            {
                QuestionList.Add(new QuestionModel()
                {
                    Id = question.Id,
                    QuestionText = question.QuestionTxt, 
                    Category = question.Category, 
                    AnswerOptions = question.AnswerOptions, 
                    CorrectAnswer = question.CorrectAnswer
                });
            }
            //------------------------------------Adderar alla kategorier till Comboboxen---------------------------------------------------------------
            

            var allCategories = _categoryRepo.GetAllCategories();

            foreach (var category in allCategories)
            {
                CategoryList.Add(new CategoryModel()
                {
                    CategoryName = category.CategoryName
                });
            }
        }
//----------------------------------------------------------------------------------------------------------------------------------

        private void AllQuestionsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllQuestionsListView.SelectedItem != null)
            {
                var selectedItem = AllQuestionsListView.SelectedItem as QuestionModel;

                QuestionTxtBox.Text = selectedItem.QuestionText;

                // Visa AnswerOptions i respektive TextBoxar
                if (selectedItem.AnswerOptions.Count >= 1)
                    AnswerOption_1_TxtBox.Text = selectedItem.AnswerOptions[0];

                if (selectedItem.AnswerOptions.Count >= 2)
                    AnswerOption_2_TxtBox.Text = selectedItem.AnswerOptions[1];

                if (selectedItem.AnswerOptions.Count >= 3)
                    AnswerOption_3_TxtBox.Text = selectedItem.AnswerOptions[2];

                CorrectAnswertxtBox.Text = selectedItem.CorrectAnswer;

            }
        }


    private void AddBtn_OnClick(object sender, RoutedEventArgs e)
        {
            string question = QuestionTxtBox.Text;
            string category = CatComboBox.Text;

            string answerOption1 = AnswerOption_1_TxtBox.Text;
            string answerOption2 = AnswerOption_2_TxtBox.Text;
            string answerOption3 = AnswerOption_3_TxtBox.Text;
            List<string> answerOptions = new List<string>{answerOption1, answerOption2, answerOption3};

            string correctAnswer = CorrectAnswertxtBox.Text;

            if (correctAnswer != answerOption1 && correctAnswer != answerOption2 && correctAnswer != answerOption3)
            {
                MessageBox.Show($"The correct answer must match one of the three alternatives");
                return;
            }


            QuestionModel questionModel = new QuestionModel();
            questionModel.QuestionText = question;
            questionModel.Category = category;
            questionModel.AnswerOptions = answerOptions;
            questionModel.CorrectAnswer = correctAnswer;


            var newQuestion = new QuestionRecord("", question, category, answerOptions, correctAnswer);



            _questionRepo.AddQuestion(newQuestion);
            QuestionList.Clear();

            var allQuestions = _questionRepo.GetAllQuestions();

            foreach (var quest in allQuestions)
            {
                QuestionList.Add(new QuestionModel()
                {
                    Id = quest.Id,
                    QuestionText = quest.QuestionTxt,
                    Category = quest.Category,
                    AnswerOptions = quest.AnswerOptions,
                    CorrectAnswer = quest.CorrectAnswer
                });
            }

            //QuestionList.Add(questionModel);


            MessageBox.Show($"You have added a new question");

            QuestionTxtBox.Clear();
            AnswerOption_1_TxtBox.Clear();
            AnswerOption_2_TxtBox.Clear();
            AnswerOption_3_TxtBox.Clear();
            CorrectAnswertxtBox.Clear();

        }


        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var questionToUpdate = AllQuestionsListView.SelectedItem as QuestionModel;
            if (questionToUpdate == null)
            {
                MessageBox.Show($"You have to select which question you want to update");
                return;
            }

            string updatedQuestion = QuestionTxtBox.Text;
            string updatedCategory = CatComboBox.Text;

            string newOption1 = AnswerOption_1_TxtBox.Text;
            string newOption2 = AnswerOption_2_TxtBox.Text;
            string newOption3 = AnswerOption_3_TxtBox.Text;
            List<string> updatedOptions = new List<string>{newOption1, newOption2, newOption3};

            string updatedCorrectAnswer = CorrectAnswertxtBox.Text;

            string sameId = questionToUpdate.Id;

            QuestionModel questionModel = new QuestionModel();
            questionModel.Id = sameId;
            questionModel.QuestionText = updatedQuestion;
            questionModel.Category = updatedCategory;
            questionModel.AnswerOptions = updatedOptions;
            questionModel.CorrectAnswer = updatedCorrectAnswer;

            _questionRepo.UpdateQuestion(questionToUpdate.Id, updatedQuestion, updatedCategory, updatedOptions, updatedCorrectAnswer);
            QuestionList.Remove(questionToUpdate);
            QuestionList.Add(questionModel);
        }


        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var questionToDelete = AllQuestionsListView.SelectedItem as QuestionModel;

            if (questionToDelete == null)
            {
                MessageBox.Show($"You have to select a question you want to delete");
                return;
            }

            MessageBox.Show($"You have deleted {questionToDelete.QuestionText}");
            _questionRepo.DeleteQuestion(questionToDelete.Id);

            QuestionList.Remove(questionToDelete);

            QuestionTxtBox.Clear();
            AnswerOption_1_TxtBox.Clear();
            AnswerOption_2_TxtBox.Clear();
            AnswerOption_3_TxtBox.Clear();
            CorrectAnswertxtBox.Clear();

        }

        private void CategoryRepository_CategoryListChanged()
        {
            CatComboBox.ItemsSource = _categoryRepo.GetAllCategories();
        }
    }
}
