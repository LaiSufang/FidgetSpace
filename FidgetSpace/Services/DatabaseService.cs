using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FidgetSpace.Models;

namespace FidgetSpace.Services
{
    public class DatabaseService
    {
        private const string DB_NAME = "fidget-space.db3";
        private readonly SQLiteAsyncConnection _connection;

        public DatabaseService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            _connection.CreateTableAsync<User>().Wait();

        }

        public async Task<List<User>> GetUsers()
        {
            return await _connection.Table<User>().ToListAsync();
        }
        public async Task<User> GetById(int id)
        {
            return await _connection.Table<User>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Create(User users)
        {
            await _connection.InsertAsync(users);
        }

        public async Task Update(User users)
        {
            await _connection.UpdateAsync(users);
        }

        public async Task Delete(User users)
        {
            await _connection.DeleteAsync(users);
        }
    }
}
