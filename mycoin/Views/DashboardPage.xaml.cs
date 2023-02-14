using mycoin.Extensions;
using mycoin.Helpers;
using mycoin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

            title.Text = GlobalConstants.LangGUI.GetValueOrDefault("Synchronisation", "Synchronisation");
            subTitle.Text = GlobalConstants.LangGUI.GetValueOrDefault("Synchronisation", "Synchronisation");
            subText.Text = GlobalConstants.LangGUI.GetValueOrDefault("Synchronisation from web server tables and Groups and program substances",
                "Synchronisation from web server tables and Groups and program substances");
        }

        async private void loadDB()
        {
            try
            {
                //Old Module Count
                int moduleCount = App.Database.GetAllModulesAsync().Result.Count();

                await App.Database.DeleteAllNotesAsync();
                await App.Database.DeleteAllLanguagesAsync();
                await App.Database.DeleteAllLanguageGUIAsync();
                await App.Database.DeleteAllSubstanceTextAsync();
                await App.Database.DeleteAllGroupTextAsync();
                await App.Database.DeleteAllAppSettingAsync();
                await App.Database.DeleteAllModulesAsync();
                //pull the data from api
                List<Note> allNotes = await HttpHelper.Instance.PostContentAsync<List<Note>>(ApiURLs.LoadAllDB, App.Userdata);
                List<Note> response = await HttpHelper.Instance.PostContentAsync<List<Note>>(ApiURLs.LoadDB, App.Userdata);
                List<Language> langRes = await HttpHelper.Instance.PostContentAsync<List<Language>>(ApiURLs.LoadLanguages, App.Userdata);
                List<LanguageGUI> langGUIRes = await HttpHelper.Instance.PostContentAsync<List<LanguageGUI>>(ApiURLs.LoadLanguageGUI, App.Userdata);
                List<SubstanceText> subText = await HttpHelper.Instance.PostContentAsync<List<SubstanceText>>(ApiURLs.LoadSubstanceText, App.Userdata);
                List<GroupText> groupText = await HttpHelper.Instance.PostContentAsync<List<GroupText>>(ApiURLs.LoadSubstanceGroupText, App.Userdata);
                List<Module> allModules = await HttpHelper.Instance.PostContentAsync<List<Module>>(ApiURLs.LoadModules, App.Userdata);

                //new Module Count
                Constants appConstants = await App.Database.GetConstantsAsync();
                if (appConstants == null)
                {
                    await App.Database.SaveConstantsAsync(new Constants()
                    {
                        NewModuleCounts = allModules.Count() - moduleCount
                    });
                }
                else
                {
                    appConstants.NewModuleCounts = appConstants.NewModuleCounts + (allModules.Count() - moduleCount);
                    await App.Database.UpdateConstantsAsync(appConstants);
                }
                GlobalConstants.NewModuleCount = App.Database.GetConstantsAsync().Result.NewModuleCounts;

                //Selected Groups from Private User Questions
                if (GlobalConstants.GroupIds.Count > 0)
                {
                    //response = response.FindAll(n => GlobalConstants.GroupIds.Contains(n.GroupNumber));
                    response.AddRange(allNotes.FindAll(n => GlobalConstants.GroupIds.Contains(n.GroupNumber)));
                    response = response.GroupBy(s => s.SubstanceID).Select(g => g.First()).ToList();
                }

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
                foreach(Module module in allModules)
                {
                    await App.Database.SaveModuleAsync(module);
                }

                //await Task.Delay(500);
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
