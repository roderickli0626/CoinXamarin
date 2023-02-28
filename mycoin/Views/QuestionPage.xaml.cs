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
using XF.Material.Forms.Resources;
using XF.Material.Forms.UI.Dialogs.Configurations;
using XF.Material.Forms.UI.Dialogs;

namespace mycoin.Views
{
    public partial class QuestionPage : BasePage
    {
        public IList<Question> Monkeys { get; private set; } = new List<Question>();
        public ViewCell lastCell;
        public List<Question> remainQuestions = new List<Question>();
        int pageNumber = 0;
        int totalPages = 0;
        public QuestionPage()
        {
            InitializeComponent();
            LoadQuestionDB();
            QuestionTitle.Text = GlobalConstants.LangGUI.GetValueOrDefault("Questions and Answers", "Questions and Answers");
            NextButton.Text = GlobalConstants.LangGUI.GetValueOrDefault("NEXT", "NEXT");
            FinishButton.Text = GlobalConstants.LangGUI.GetValueOrDefault("FINISH", "FINISH");
        }

        //void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    Grid gridCell = (Grid)sender;
        //    ((Image)gridCell.Children.Last()).IsVisible = !(((Image)gridCell.Children.Last()).IsVisible);
        //    Question selectedItem = e.SelectedItem as Question;

        //    if (gridCell.Children.Last().IsVisible)
        //    {
        //        GlobalConstants.GroupIds.Add(selectedItem.GroupNumber ?? 0);
        //    }
        //    else
        //    {
        //        GlobalConstants.GroupIds.Remove(selectedItem.GroupNumber ?? 0);
        //    }

        //    if (GlobalConstants.GroupIds.Count() > 0 && GlobalConstants.Questions.Count() > 0)
        //    {
        //        NextButton.IsVisible = true;
        //    }
        //    else
        //    {
        //        NextButton.IsVisible = false;
        //    }
        //}

        //void OnListViewItemTapped(object sender, ItemTappedEventArgs e)
        //{
        //    Question tappedItem = e.Item as Question;
        //}



        async private void LoadQuestionDB()
        {
            try
            {
                await App.Database.DeleteAllQuestionsAsync();
                //pull the Questions data from api
                List<Question> response = await HttpHelper.Instance.PostContentAsync<List<Question>>(ApiURLs.LoadQuestions, App.Userdata);

                //Question is empty
                if (response.Count == 0)
                {
                    var alertDialogConfiguration = new MaterialAlertDialogConfiguration()
                    {
                        BackgroundColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.SURFACE),
                        TitleTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_SURFACE),
                        MessageTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_SURFACE),
                        //TintColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_BACKGROUND),
                        TintColor = Color.FromHex("#018BD3"),
                        CornerRadius = 30,
                        ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32),
                        ButtonAllCaps = false
                    };
                    await MaterialDialog.Instance.ConfirmAsync(GlobalConstants.LangGUI.GetValueOrDefault("No questions are currently available. Try again later.", "No questions are currently available. Try again later."),
                    GlobalConstants.LangGUI.GetValueOrDefault("Warning", "Warning"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"), "", alertDialogConfiguration);

                    //Close the app
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    //Minimize app
                    //System.Threading.Thread.CurrentThread.Abort();
                }

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
                    remainQuestions.Add(question);
                }
                totalPages = questionList.Count / 10 + 1;
                pageNumber = 1;
                
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
            if (remainQuestions.Count() > 10)
            {
                QuestionList.ItemsSource = remainQuestions.GetRange(0, 10).ToArray();
                remainQuestions = remainQuestions.GetRange(10, remainQuestions.Count() - 10);
                FinishButton.IsVisible = false;
                //NextButton.IsVisible = false;
                SkipButton.Text = "Skip";
                SkipButton.IsEnabled = true;
                QuestionSubTitle.Text = GlobalConstants.LangGUI.GetValueOrDefault("Please answer some questions below", "Please answer some questions below");
            }
            else
            {
                QuestionList.ItemsSource = remainQuestions.ToArray();
                remainQuestions.RemoveRange(0, remainQuestions.Count());
                FinishButton.IsVisible = true;
                NextButton.IsVisible = false;
                SkipButton.Text = "";
                SkipButton.IsEnabled = false;
                QuestionSubTitle.Text = GlobalConstants.LangGUI.GetValueOrDefault("Finish your process", "Finish your process");
            }
            PageCounts.Text = "" + pageNumber + "/" + totalPages;
        }

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            pageNumber++;
            PreviousButton.Source = "ic_left_arrow_white.png";
            PreviousButton.IsEnabled = true;
            loadItemSource();
        }

        private void FinishButton_Clicked(object sender, EventArgs e)
        {
            QuestionOption questionOption = new QuestionOption();
            questionOption.UserID = App.Userdata.userid;
            questionOption.SelectedQuestionList = string.Join(",", GlobalConstants.GroupIds);

            App.Database.SaveSelectedQuestionAsync(questionOption);
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
                ((ViewCell)grid.Parent).View.BackgroundColor = Color.LightGray;
            }
            else
            {
                GlobalConstants.GroupIds.Remove(selectedItem.GroupNumber ?? 0);
                ((ViewCell)grid.Parent).View.BackgroundColor = Color.White;
            }

            if (GlobalConstants.GroupIds.Count() > 0 && remainQuestions.Count() > 0)
            {
                NextButton.IsVisible = true;
            }
            else
            {
                NextButton.IsVisible = false;
            }
        }

        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            if (lastCell != null)
                lastCell.View.BackgroundColor = Color.Transparent;
            var viewCell = (ViewCell)sender;
            if (viewCell.View != null)
            {
                viewCell.View.BackgroundColor = Color.White;
                lastCell = viewCell;
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            pageNumber++;
            PreviousButton.Source = "ic_left_arrow_white.png";
            PreviousButton.IsEnabled = true;
            loadItemSource();
            //GlobalConstants.GroupIds.Clear();
            //App.Current.MainPage = new NavigationPage(new DashboardPage());
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            pageNumber--;
            if (pageNumber < 2)
            {
                PreviousButton.Source = "ic_left_arrow_blue.png";
                PreviousButton.IsEnabled = false;
            }
            if (pageNumber < 1) return;
            PageCounts.Text = "" + pageNumber + "/" + totalPages;
            remainQuestions = GlobalConstants.Questions.GetRange((pageNumber - 1) * 10, GlobalConstants.Questions.Count() - (pageNumber - 1) * 10);
            loadItemSource();
        }
    }
}