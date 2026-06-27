using System;
using System.Collections.Generic;

namespace CyberBot
{
    public class TaskManager
    {
        private List<Task> tasks = new List<Task>();
        private int idCounter = 1;

        
        // ADD TASK
        
        public Task AddTask(string title, string desc, DateTime due)
        {
            Task t = new Task
            {
                TaskId = idCounter++,
                Title = title,
                Description = desc,
                ReminderDate = due,
                IsCompleted = false
            };

            tasks.Add(t);

            ActivityLogger.Instance.Log($"Task added: {title}");

            return t;
        }

        // GET TASKS
        
        public List<Task> GetTasks()
        {
            return tasks;
        }

       
        // DELETE TASK
        
        public void DeleteTask(int id)
        {
            tasks.RemoveAll(t => t.TaskId == id);
            ActivityLogger.Instance.Log($"Task deleted: {id}");
        }

       
        // COMPLETE TASK
        
        public void CompleteTask(int id)
        {
            var task = tasks.Find(t => t.TaskId == id);

            if (task != null)
            {
                task.IsCompleted = true;
                ActivityLogger.Instance.Log($"Task completed: {id}");
            }
        }

     
        // UPDATE TASK
        
        public void UpdateTask(int id, string newTitle, string newDesc)
        {
            var task = tasks.Find(t => t.TaskId == id);

            if (task != null)
            {
                task.Title = newTitle;
                task.Description = newDesc;

                ActivityLogger.Instance.Log($"Task updated: {id}");
            }
        }
    }
}
