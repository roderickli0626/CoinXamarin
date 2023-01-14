using mycoin.Extensions;
using mycoin.Helpers;
using mycoin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mycoin.Views
{
    public partial class QuestionPage : BasePage
    {
        public IList<Question> Monkeys { get; private set; } = new List<Question>();
        public QuestionPage()
        {
            InitializeComponent();
            LoadQuestionDB();
            QuestionTitle.Text = GlobalConstants.LangGUI.GetValueOrDefault("Questions and Answers", "Questions and Answers");
            NextButton.Text = GlobalConstants.LangGUI.GetValueOrDefault("NEXT", "NEXT");
            FinishButton.Text = GlobalConstants.LangGUI.GetValueOrDefault("FINISH", "FINISH");
        }

        void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Grid gridCell = (Grid)sender;
            ((Image)gridCell.Children.Last()).IsVisible = !(((Image)gridCell.Children.Last()).IsVisible);
            Question selectedItem = e.SelectedItem as Question;

            if (gridCell.Children.Last().IsVisible)
            {
                GlobalConstants.GroupIds.Add(selectedItem.GroupNumber ?? 0);
            }
            else
            {
                GlobalConstants.GroupIds.Remove(selectedItem.GroupNumber ?? 0);
            }

            if (GlobalConstants.GroupIds.Count() > 0 && GlobalConstants.Questions.Count() > 0)
            {
                NextButton.IsVisible = true;
            }
            else
            {
                NextButton.IsVisible = false;
            }
        }

        void OnListViewItemTapped(object sender, ItemTappedEventArgs e)
        {
            Question tappedItem = e.Item as Question;
        }



        async private void LoadQuestionDB()
        {
            try
            {
                await App.Database.DeleteAllQuestionsAsync();
                //pull the Questions data from api
                List<Question> response = await HttpHelper.Instance.PostContentAsync<List<Question>>(ApiURLs.LoadQuestions, App.Userdata);

                foreach (Question item in response)
                {
                    await App.Database.SaveQuestionAsync(item);
                }

                // Save Questions to GlobalConstant according to the language
                AppSettings settings = App.Database.GetSettingsAsync().Result;
                GlobalConstants.Questions.Clear();
                List<Question> questionList = App.Database.GetQuestionsByLangAsync((settings != null && settings.languageNumber != 0) ? settings.languageNumber : (App.Userdata.languageid ?? 0)).Result;
                foreach (Question question in questionList)
                {
                    GlobalConstants.Questions.Add(question);
                }
                GlobalConstants.QuestionPageNumber = 0;

                loadItemSource();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new NotImplementedException();
            }
        }

        void loadItemSource()
        {
            if (GlobalConstants.Questions.Count() > 10)
            {
                QuestionList.ItemsSource = GlobalConstants.Questions.GetRange(0, 10).ToArray();
                GlobalConstants.Questions = GlobalConstants.Questions.GetRange(10, GlobalConstants.Questions.Count() - 10);
                FinishButton.IsVisible = false;
                //NextButton.IsVisible = false;
            }
            else
            {
                QuestionList.ItemsSource = GlobalConstants.Questions.ToArray();
                GlobalConstants.Questions.RemoveRange(0, GlobalConstants.Questions.Count());
                FinishButton.IsVisible = true;
                NextButton.IsVisible = false;
            }
        }

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            loadItemSource();
        }

        private void FinishButton_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new DashboardPage());
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Label label = sender as Label;
            Grid grid = (Grid)label.Parent;
            ((Image)grid.Children.Last()).IsVisible = !(((Image)grid.Children.Last()).IsVisible);
            Question selectedItem = label.BindingContext as Question;

            if (grid.Children.Last().IsVisible)
            {
                GlobalConstants.GroupIds.Add(selectedItem.GroupNumber ?? 0);
            }
            else
            {
                GlobalConstants.GroupIds.Remove(selectedItem.GroupNumber ?? 0);
            }

            if (GlobalConstants.GroupIds.Count() > 0 && GlobalConstants.Questions.Count() > 0)
            {
                NextButton.IsVisible = true;
            }
            else
            {
                NextButton.IsVisible = false;
            }
        }
    }
}