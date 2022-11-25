using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using mycoin.Models;

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