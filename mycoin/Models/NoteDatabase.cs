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
        }

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