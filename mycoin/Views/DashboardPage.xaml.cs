using mycoin.Helpers;
using mycoin.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace mycoin.Views
{
    public partial class DashboardPage : BasePage
    {
        Task rotate;
        bool endRotate;
        public DashboardPage()
        {
            InitializeComponent();
            loadDB();
            rotate = RotateImageContinously();
            endRotate = false;

        }

        async private void loadDB()
        {
            try
            {
                await App.Database.DeleteAllNotesAsync();
                await App.Database.DeleteAllLanguagesAsync();
                await App.Database.DeleteAllLanguageGUIAsync();
                await App.Database.DeleteAllSubstanceTextAsync();
                await App.Database.DeleteAllGroupTextAsync();
                await App.Database.DeleteAllAppSettingAsync();
                //pull the data from api
                List<Note> response = await HttpHelper.Instance.PostContentAsync<List<Note>>(ApiURLs.LoadDB, App.Userdata);
                List<Language> langRes = await HttpHelper.Instance.PostContentAsync<List<Language>>(ApiURLs.LoadLanguages, App.Userdata);
                List<LanguageGUI> langGUIRes = await HttpHelper.Instance.PostContentAsync<List<LanguageGUI>>(ApiURLs.LoadLanguageGUI, App.Userdata);
                List<SubstanceText> subText = await HttpHelper.Instance.PostContentAsync<List<SubstanceText>>(ApiURLs.LoadSubstanceText, App.Userdata);
                List<GroupText> groupText = await HttpHelper.Instance.PostContentAsync<List<GroupText>>(ApiURLs.LoadSubstanceGroupText, App.Userdata);

                foreach(Note item in response)
                {
                    item.Isfavorite = false;
                    await App.Database.SaveNoteAsync(item);
                }
                foreach(Language lang in langRes)
                {
                    await App.Database.SaveLanguageAsync(lang);
                }
                foreach(LanguageGUI lanGUI in langGUIRes)
                {
                    await App.Database.SaveLanguageGUIAsync(lanGUI);
                }
                foreach(SubstanceText substanceText in subText)
                {
                    await App.Database.SaveSubstanceTextAsync(substanceText);
                }
                foreach(GroupText gText in groupText)
                {
                    await App.Database.SaveGroupTextAsync(gText);
                }

                await Task.Delay(1000);
                endRotate = true;
                App.Current.MainPage = new NavigationPage(new MainDashboardPage());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new NotImplementedException();
            }
        }

        //async private void Button_Clicked(object sender, EventArgs e)
        //{
        //    Note notes = await App.Database.GetNoteAsync(1);
        //    Console.WriteLine(notes);
        //}

        public async Task RotateImageContinously()
        {
            while (!endRotate) // a CancellationToken in real life ;-)
            {
                for (int i = 1; i < 7; i++)
                {
                    if (LoadingImage.Rotation >= 360f) LoadingImage.Rotation = 0;
                    await LoadingImage.RotateTo(i * (360 / 6), 100, Easing.Linear);
                }
            }
        }
    }
}
