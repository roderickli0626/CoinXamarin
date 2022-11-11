using mycoin.Helpers;
using mycoin.Models;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace mycoin.Views
{
    public partial class DashboardPage : BasePage
    {
        public DashboardPage()
        {
            InitializeComponent();
            loadDB();
        }

        async private void loadDB()
        {
            try
            {
                //pull the data from api
                List<Note> response = await HttpHelper.Instance.PostContentAsync<List<Note>>(ApiURLs.LoadDB, App.Userdata);

                foreach(Note item in response)
                {
                    await App.Database.SaveNoteAsync(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new NotImplementedException();
            }
        }

        async private void Button_Clicked(object sender, EventArgs e)
        {
            Note notes = await App.Database.GetNoteAsync(1);
            Console.WriteLine(notes);
        }
    }
}
