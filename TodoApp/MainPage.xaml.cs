﻿using System.Collections.ObjectModel;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp
{
    public partial class MainPage : ContentPage
    {
        private DatabaseService _databaseService;
        public ObservableCollection<TaskItem> Tasks { get; set; }

        public MainPage()
        {
            InitializeComponent();
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tasks.db");
            _databaseService = new DatabaseService(dbPath);
            Tasks = new ObservableCollection<TaskItem>(_databaseService.GetTasks());
            BindingContext = this;
        }

        private void OnTaskCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var task = checkBox?.BindingContext as TaskItem;
            if (task != null)
            {
                task.IsCompleted = e.Value;
                _databaseService.SaveTask(task); // Зберігаємо зміни у базі даних
            }
        }
        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            string title = await DisplayPromptAsync("New Task", "Enter task title:");
            if (string.IsNullOrEmpty(title))
                return;

            // Відкриваємо модальне вікно для вибору дати та часу
            var dateTime = await ShowDateTimePicker();

            if (dateTime != null)
            {
                TaskItem newTask = new TaskItem
                {
                    Title = title,
                    ReminderTime = dateTime.Value,
                    IsCompleted = false
                };
                _databaseService.SaveTask(newTask);
                Tasks.Add(newTask);
            }
        }

        private async Task<DateTime?> ShowDateTimePicker(DateTime? initialDateTime = null)
        {
            var datePicker = new DatePicker
            {
                Date = initialDateTime?.Date ?? DateTime.Today,
                MinimumDate = DateTime.Today
            };
            var timePicker = new TimePicker
            {
                Time = initialDateTime?.TimeOfDay ?? TimeSpan.Zero
            };

            var stackLayout = new VerticalStackLayout
            {
                Children = { new Label { Text = "Select Date:" }, datePicker, new Label { Text = "Select Time:" }, timePicker }
            };

            var modalPage = new ContentPage
            {
                Content = new VerticalStackLayout
                {
                    Padding = 20,
                    Children =
                    {
                        stackLayout,
                        new Button
                        {
                            Text = "OK",
                            Command = new Command(async () =>
                            {
                                await Application.Current.MainPage.Navigation.PopModalAsync();
                            })
                        }
                    }
                }
            };

            await Application.Current.MainPage.Navigation.PushModalAsync(modalPage);

            return await Task.Run(() =>
            {
                while (Application.Current.MainPage.Navigation.ModalStack.Contains(modalPage))
                {
                    Task.Delay(100).Wait();
                }
                return (DateTime?)datePicker.Date.Add(timePicker.Time);
            });
        }

        private Command<int> _deleteTaskCommand;
        public Command<int> DeleteTaskCommand =>
            _deleteTaskCommand ??= new Command<int>(id =>
            {
                try 
                {
                    _databaseService.DeleteTask(id);
                    Tasks.Remove(Tasks.First(t => t.Id == id));
                }
                catch { }
            });


        private Command<TaskItem> _editTaskCommand;
        public Command<TaskItem> EditTaskCommand =>
            _editTaskCommand ??= new Command<TaskItem>(async task => await EditTaskAsync(task));

        private async Task EditTaskAsync(TaskItem task)
        {
            // Запитуємо новий заголовок
            string newTitle = await DisplayPromptAsync("Edit Task", "Enter new title:", initialValue: task.Title);
            if (string.IsNullOrEmpty(newTitle))
                return;

            // Запитуємо нову дату й час
            var newDateTime = await ShowDateTimePicker(task.ReminderTime);
            if (newDateTime == null)
                return;

            // Оновлюємо дані завдання
            task.Title = newTitle;
            task.ReminderTime = newDateTime.Value;

            // Зберігаємо зміни в базі даних
            _databaseService.SaveTask(task);

            // Оновлюємо колекцію, щоб зміни відобразилися
            var index = Tasks.IndexOf(task);
            Tasks.RemoveAt(index);
            Tasks.Insert(index, task);
        }




    }
}