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
    }
}