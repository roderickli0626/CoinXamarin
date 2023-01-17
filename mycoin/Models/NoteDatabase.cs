using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using mycoin.Models;
using System;

namespace mycoin.Data
{
    public class NoteDatabase
    {
        readonly SQLiteAsyncConnection database;

        public NoteDatabase(string dbPath)
        {

            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Note>().Wait();
            database.CreateTableAsync<Userdata>().Wait();
            database.CreateTableAsync<Calendar>().Wait();
            database.CreateTableAsync<AppSettings>().Wait();
            database.CreateTableAsync<Language>().Wait();
            database.CreateTableAsync<LanguageGUI>().Wait();
            database.CreateTableAsync<SubstanceText>().Wait();
            database.CreateTableAsync<GroupText>().Wait();
            database.CreateTableAsync<Question>().Wait();
            database.CreateTableAsync<QuestionOption>().Wait();
        }

        //QuestionOption
        public Task<int> DeleteAllSelectedQuestionsAsync()
        {
            // Delete all SelectedQuestions.
            return database.DeleteAllAsync<QuestionOption>();
        }
        public Task<int> DeleteSelectedQuestionsByUserAsync(QuestionOption option)
        {
            // Delete a SelectedQuestion.
            return database.DeleteAsync(option);
        }
        public Task<QuestionOption> GetSelectedQuestionsByUserAsync(int userID)
        {
            //Get SelectedQuestions By UserID.
            return database.Table<QuestionOption>().Where(s => s.UserID == userID).FirstOrDefaultAsync();
        }
        public Task<int> SaveSelectedQuestionAsync(QuestionOption data)
        {
            //Save a new SelectedQuestions
            return database.InsertAsync(data);
        }

        //Questions
        public Task<int> DeleteAllQuestionsAsync()
        {
            // Delete all Questions.
            return database.DeleteAllAsync<Question>();
        }
        public Task<List<Question>> GetQuestionsByLangAsync(int languageNumber)
        {
            //Get Questions By Language.
            return database.Table<Question>().Where(s => s.LanguageNumber == languageNumber).ToListAsync();
        }
        public Task<int> SaveQuestionAsync(Question data)
        {
            //Save a new Question
            return database.InsertAsync(data);
        }

        //GroupText
        public Task<int> DeleteAllGroupTextAsync()
        {
            // Delete all GroupText.
            return database.DeleteAllAsync<GroupText>();
        }
        public Task<List<GroupText>> GetGroupTextByLanguageAsync(int languageNumber)
        {
            //Get GroupTexts by Language.
            return database.Table<GroupText>().Where(g => g.Language == languageNumber).ToListAsync();
        }
        public Task<int> SaveGroupTextAsync(GroupText data)
        {
            //Save a new GroupText
            return database.InsertAsync(data);
        }

        //SubstanceText
        public Task<int> DeleteAllSubstanceTextAsync()
        {
            // Delete all SubstanceText.
            return database.DeleteAllAsync<SubstanceText>();
        }
        public Task<List<SubstanceText>> GetSubstanceTextByLangAsync(int languageNumber)
        {
            //Get SubstanceTexts By Language.
            return database.Table<SubstanceText>().Where(s => s.Language == languageNumber).ToListAsync();
        }
        public Task<int> SaveSubstanceTextAsync(SubstanceText data)
        {
            //Save a new SubstanceText
            return database.InsertAsync(data);
        }


        //LanguageGUI
        public Task<int> DeleteAllLanguageGUIAsync()
        {
            // Delete all LanguageGUI.
            return database.DeleteAllAsync<LanguageGUI>();
        }
        public Task<List<LanguageGUI>> GetLanguageGUIByLanguageAsync(int languageNumber)
        {
            //Get LanguageGUI by Language.
            return database.Table<LanguageGUI>().Where(l => l.LanguageNumber == languageNumber).ToListAsync();
        }
        public Task<int> SaveLanguageGUIAsync(LanguageGUI data)
        {
            //Save a new LanguageGUI
            return database.InsertAsync(data);
        }

        //Language
        public Task<int> DeleteAllLanguagesAsync()
        {
            // Delete all Languages.
            return database.DeleteAllAsync<Language>();
        }
        public Task<List<Language>> GetAllLanguagesAsync()
        {
            //Get all Languages.
            return database.Table<Language>().ToListAsync();
        }
        public Task<int> SaveLanguageAsync(Language data)
        {
            //Save a new Languages
            return database.InsertAsync(data);
        }

        // Settings
        public Task<int> DeleteAllAppSettingAsync()
        {
            // Delete all AppSettings.
            return database.DeleteAllAsync<AppSettings>();
        }
        public Task<AppSettings> GetSettingsAsync()
        {
            //Get Saved Settings
            return database.Table<AppSettings>().FirstOrDefaultAsync();
        }
        public Task<int> SaveSettingsAsync(AppSettings data)
        {
            //Save a new Settings
            return database.InsertAsync(data);
        }

        //Calendar
        public Task<int> SaveCalendarAsync(Calendar calendar)
        {
            // Save a new calendar.
            return database.InsertAsync(calendar);
        }
        public Task<List<Calendar>> GetAllCalendarsAsync()
        {
            //Get all calendars.
            return database.Table<Calendar>().ToListAsync();
        }
        public Task<List<Calendar>> GetCalendarsAsync(DateTime startDate)
        {
            //Get all calendars on startDate.
            return database.Table<Calendar>().Where(c => c.startDate == startDate).ToListAsync();
        }

        public Task<Calendar> GetCalendarAsync(int id)
        {
            // Get a specific calendar.
            return database.Table<Calendar>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> UpdateCalendarAsync(Calendar calendar)
        {
            // Update a calendar.
            return database.UpdateAsync(calendar);
        }

        public Task<int> DeleteCalendarAsync(Calendar calendar)
        {
            // Delete a calendar.
            return database.DeleteAsync(calendar);
        }

        public Task<int> DeleteAllCalendarsAsync(DateTime startDate)
        {
            // Delete all calendars on startDate.

            return database.Table<Calendar>().Where(c => c.startDate == startDate).DeleteAsync();
        }

        //UserData
        public Task<Userdata> GetUserdataAsync()
        {
            //Get Saved UserInfo
            return database.Table<Userdata>().FirstOrDefaultAsync();
        }
        public Task<int> SaveUserdataAsync(Userdata data)
        {
            //Save a new UserInfo
            return database.InsertAsync(data);
        }
        public Task<int> DeleteAllUserdataAsync()
        {
            // Delete all UserInfo.
            return database.DeleteAllAsync<Userdata>();
        }
        public Task<int> UpdateUserdataAsync(Userdata data)
        {
            // Update a Userdata.
            return database.UpdateAsync(data);
        }

        //Note (Substances)
        public Task<List<Note>> GetNotesAsync()
        {
            //Get all notes.
            return database.Table<Note>().ToListAsync();
        }

        public Task<Note> GetNoteAsync(int id)
        {
            // Get a specific note.
            return database.Table<Note>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveNoteAsync(Note note)
        {
            // Save a new note.
            return database.InsertAsync(note);
        }

        public Task<int> DeleteNoteAsync(Note note)
        {
            // Delete a note.
            return database.DeleteAsync(note);
        }

        public Task<int> DeleteAllNotesAsync()
        {
            // Delete all notes.
            return database.DeleteAllAsync<Note>();
        }

        public Task<int> UpdateNoteAsync(Note note)
        {
            // Update a note.
            return database.UpdateAsync(note);
        }
    }
}