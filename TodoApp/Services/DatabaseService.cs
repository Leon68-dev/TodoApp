using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Services
{
    public class DatabaseService
    {
        private SQLiteConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<TaskItem>();

            var v = _database.Table<TaskItem>();
            var cnt = v.Count();

            //foreach (var item in v) 
            //{
            //    DeleteTask(item.Id);
            //}
        }

        public List<TaskItem> GetTasks()
        {
            return _database.Table<TaskItem>().OrderBy(t => t.ReminderTime).ToList();
        }

        public int SaveTask(TaskItem task)
        {
            if(task.Id != 0)
                return _database.Update(task);
            else
                return _database.Insert(task);
        }

        public int DeleteTask(int id)
        {
            return _database.Delete<TaskItem>(id);
        }
    }
}
