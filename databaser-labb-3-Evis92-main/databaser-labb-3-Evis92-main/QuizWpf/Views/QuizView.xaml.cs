using Common.DTOs;
using DataAccess.Services;
using MongoDB.Bson;
using QuizWpf.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace QuizWpf.Views
{
    /// <summary>
    /// Interaction logic for QuizView.xaml
    /// </summary>
    public partial class QuizView : UserControl
    {
        private readonly QuizRepositories _quizRepo;

        public ObservableCollection<QuizModel> QuizList { get; set; } = new();

        public QuizModel SelectedQuiz { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }


        public List<string> QuestionsList { get; set; }



        public QuizView()
        {
            InitializeComponent();
             
            DataContext = this;

            _quizRepo = new QuizRepositories();


            //---------------------- Fyller ListBoxen med Quiz----------------------------------------------------------

            var allQuizzes = _quizRepo.GetAllQuizzes();

            foreach (var quiz in allQuizzes)
            {
                QuizList.Add(new QuizModel()
                {
                    Id = quiz.Id,
                    QuizName = quiz.QuizName,
                    Description = quiz.Description,
                    Questions = quiz.Questions.Select(ObjectId.Parse).ToList()
                });
            }
        }

        

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            string quizName = QuizNameTxtBox.Text;
            string quizDescr = DescriptionTxtBox.Text;
            List<string> questions = new List<string>();

            QuizModel quizModel = new QuizModel();
            quizModel.QuizName = quizName;
            quizModel.Description = quizDescr;


            var newQuiz = new QuizRecord("", quizName, quizDescr, questions);

            _quizRepo.AddQuiz(newQuiz);

            //QuizList.Add(quizModel);

            QuizList.Clear();
            var allQuizzes = _quizRepo.GetAllQuizzes();

            foreach (var quiz in allQuizzes)
            {
                QuizList.Add(new QuizModel()
                {
                    Id = quiz.Id,
                    QuizName = quiz.QuizName,
                    Description = quiz.Description,
                    Questions = quiz.Questions.Select(ObjectId.Parse).ToList()
                });
            }

            MessageBox.Show($"You have added {quizModel.QuizName}");
            QuizNameTxtBox.Clear();
            DescriptionTxtBox.Clear();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {

            var quizToDelete = AllQuizzesListView.SelectedItem as QuizModel;

            if (quizToDelete == null)
            {
                MessageBox.Show($"You have to select a Quiz to delete");
                return;
            }

            MessageBox.Show($"You have deleted {quizToDelete.QuizName}");

            _quizRepo.DeleteQuiz(quizToDelete.Id);
            QuizList.Remove(quizToDelete);

            QuizNameTxtBox.Clear();
            DescriptionTxtBox.Clear();
        }

        private void AllQuizzesListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllQuizzesListView.SelectedItem != null)
            {
                var selectedItem = AllQuizzesListView.SelectedItem as QuizModel;

                QuizNameTxtBox.Text = selectedItem.QuizName;
                DescriptionTxtBox.Text = selectedItem.Description;
            }
        }
    }
}
