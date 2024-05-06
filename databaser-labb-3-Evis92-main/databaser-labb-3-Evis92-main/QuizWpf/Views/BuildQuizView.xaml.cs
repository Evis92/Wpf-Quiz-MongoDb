using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DataAccess.Services;
using MongoDB.Bson;
using QuizWpf.Models;

namespace QuizWpf.Views
{
    /// <summary>
    /// Interaction logic for BuildQuizView.xaml
    /// </summary>
    public partial class BuildQuizView : UserControl
    {

        private readonly QuizRepositories _quizRepo;
        private readonly QuestionRepositories _questionRepo;
        private readonly CategoryRepository _categoryRepo;

        public ObservableCollection<QuizModel> QuizList { get; set; } = new();

        public QuizModel SelectedQuiz { get; set; } = new();

        public string QuizName { get; set; }

        public string Description { get; set; }

        public ObservableCollection<QuestionModel> QuizQuestionsList { get; set; } = new ObservableCollection<QuestionModel>();





        public ObservableCollection<QuestionModel> QuestionList { get; set; } = new();

        public QuestionModel SelectedQuestion { get; set; } = new();

        public string QuestionTxt { get; set; }

        public string Category { get; set; }

        public List<string> AnswerOptionList { get; set; }

        public string CorrectAnswer { get; set; }





        public ObservableCollection<CategoryModel> CategoryList { get; set; } = new();

        public CategoryModel SelectedCategory { get; set; } = new();

        public string CategoryName { get; set; }





        public BuildQuizView()
        {
            InitializeComponent();

            QuizRepositories.QuizListChanged += QuizRepository_QuizListChanged;
            CategoryRepository.CategoryListChanged += CategoryRepository_CategoryListChanged;
            QuestionRepositories.QuestionListChanged += QuestionRepository_CategoryListChanged;

            DataContext = this;

            CategoryList.Insert(0, new CategoryModel() { Id = ObjectId.GenerateNewId().ToString(), CategoryName = "All Categories" });

            _quizRepo = new QuizRepositories();
            _questionRepo = new QuestionRepositories();
            _categoryRepo = new CategoryRepository();

            //--------------------Fyller FrågeListan-------------------------------------------
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

            //----------------------------------------------------------------------------------

            //-------------------Fyll Comboboxen med Kategorier!!!------------------------

            var allCategories = _categoryRepo.GetAllCategories();

            foreach (var category in allCategories)
            {
                CategoryList.Add(new CategoryModel()
                {
                    Id = category.Id,
                    CategoryName = category.CategoryName
                });
            }


            //------------------Fyll ComboBoxen med Quizzes!!!------------------------------

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


        //---------------------Ändrar ListVyn med alla Frågor efter val av Kategori i Combobox!!
        private void CategoryComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QuestionList.Clear();

            if (CategoryComboBox.SelectedItem is CategoryModel selectedCategory)
            {
                if (selectedCategory.CategoryName == "All Categories")
                {
                    // Rensa ComboBox-valet
                    CategoryComboBox.SelectedItem = null;
                }
                else
                {
                    var allQuestions = _questionRepo.GetAllQuestions(); // Använd allQuestions som källa

                    var filteredQuestions = allQuestions.Where(q => q.Category == selectedCategory.CategoryName).ToList();

                    foreach (var questionRecord in filteredQuestions)
                    {
                        QuestionList.Add(new QuestionModel()
                        {
                            Id = questionRecord.Id,
                            QuestionText = questionRecord.QuestionTxt,
                            Category = questionRecord.Category,
                            AnswerOptions = questionRecord.AnswerOptions,
                            CorrectAnswer = questionRecord.CorrectAnswer
                        });
                    }
                }
            }
            else
            {
                // Återställ till alla frågor om ingen kategori är vald
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
            }
        }



        private void QuizComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QuizQuestionsList.Clear();

            if (QuizComboBox.SelectedItem is QuizModel selectedQuiz)
            {
                var theQuiz = _quizRepo.GetQuizById(ObjectId.Parse(selectedQuiz.Id));

                // Hämta varje fråga med dess ObjectId från QuestionRepository
                foreach (var questionId in theQuiz.Questions)
                {
                    var question = _questionRepo.GetQuestionById(questionId);

                    // Lägg till frågan i QuizQuestionsList
                    QuizQuestionsList.Add(new QuestionModel()
                    {
                        Id = question.Id,
                        QuestionText = question.QuestionTxt,
                        Category = question.Category,
                        AnswerOptions = question.AnswerOptions,
                        CorrectAnswer = question.CorrectAnswer
                    });
                }
            }
        }




        private void RemoveQuestionfromQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var questionToRemove = QuizQuestionListView.SelectedItem as QuestionModel;

            if (questionToRemove == null)
            {
                MessageBox.Show($"You have to select which question you want to remove from quiz");
                return;
            }


            var quizToUpdate = _quizRepo.GetQuizById(ObjectId.Parse(SelectedQuiz.Id));

            MessageBox.Show($"You have removed '{questionToRemove.QuestionText}' from {quizToUpdate.QuizName}");


            quizToUpdate.Questions.Remove(ObjectId.Parse(questionToRemove.Id));
            _quizRepo.UpdateQuiz(quizToUpdate.Id.ToString(), quizToUpdate.QuizName, quizToUpdate.Description, quizToUpdate.Questions);


            QuizQuestionsList.Remove(questionToRemove);

        }





        private void AddQuestionToQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            //Ska lägga till en fråga till quizzet

            var questionToAdd = AllQuestionListView.SelectedItem as QuestionModel;

            if (questionToAdd == null)
            {
                MessageBox.Show($"You have to select a question you want to add");
                return;
            }

            if (QuizComboBox.SelectedItem == null)
            {
                MessageBox.Show($"You have to select which quiz you would like to update");
                return;
            }
            else
            {
                // Hämta quizzet som ska updateras
                var quizToUpdate = _quizRepo.GetQuizById(ObjectId.Parse(SelectedQuiz.Id));



                // Lägg till frågan i quizen i databasen
                quizToUpdate.Questions.Add(ObjectId.Parse(questionToAdd.Id));

                MessageBox.Show($"You have added '{questionToAdd.QuestionText}' to {quizToUpdate.QuizName}");

                _quizRepo.UpdateQuiz(quizToUpdate.Id.ToString(), quizToUpdate.QuizName, quizToUpdate.Description, quizToUpdate.Questions);

                QuizQuestionsList.Add(questionToAdd);
            }


        }



        private void SelectQuestionTxtBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SelectQuestionTxtBox.Text.ToLower();

            QuizQuestionsList.Clear();

            if (QuizComboBox.SelectedItem is QuizModel selectedQuiz)
            {
                var quiz = _quizRepo.GetQuizById(ObjectId.Parse(selectedQuiz.Id));

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    foreach (var questionId in quiz.Questions)
                    {
                        var question = _questionRepo.GetQuestionById(questionId);
                        QuizQuestionsList.Add(new QuestionModel()
                        {
                            Id = question.Id,
                            QuestionText = question.QuestionTxt,
                            Category = question.Category,
                            AnswerOptions = question.AnswerOptions,
                            CorrectAnswer = question.CorrectAnswer
                        });
                    }
                }
                else
                {
                    
                    foreach (var questionId in quiz.Questions)
                    {
                        var question = _questionRepo.GetQuestionById(questionId);

                        if (question.QuestionTxt.ToLower().Contains(searchText))
                        {
                            QuizQuestionsList.Add(new QuestionModel()
                            {
                                Id = question.Id,
                                QuestionText = question.QuestionTxt,
                                Category = question.Category,
                                AnswerOptions = question.AnswerOptions,
                                CorrectAnswer = question.CorrectAnswer
                            });
                        }
                    }
                }
            }
        }

        private void QuizRepository_QuizListChanged()
        {
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

            //QuizComboBox.ItemsSource = _quizRepo.GetAllQuizzes();
        }

        private void CategoryRepository_CategoryListChanged()
        {
            CategoryList.Clear();

            CategoryList.Insert(0, new CategoryModel() { Id = ObjectId.GenerateNewId().ToString(), CategoryName = "All Categories" });
            var allCategories = _categoryRepo.GetAllCategories();

            foreach (var category in allCategories)
            {
                CategoryList.Add(new CategoryModel()
                {
                    Id = category.Id,
                    CategoryName = category.CategoryName
                });
            }
        }

        private void QuestionRepository_CategoryListChanged()
        {
            QuestionList.Clear();
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
        }

    }
}
