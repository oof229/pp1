using System;
using System.Collections.Generic;

namespace ProjectLibrary
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public int RoleId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
    }

    public class Task
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AssignedUserId { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; }
    }

    public class ProjectManagement
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<Role> Roles { get; set; } = new List<Role>();
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<Task> Tasks { get; set; } = new List<Task>();
        public void AddUser(User user) => Users.Add(user);
        public void AddProject(Project project) => Projects.Add(project);
        public void AddTask(Task task) => Tasks.Add(task);

        public void AssignTaskToUser(int taskId, int userId)
        {
            var task = Tasks.Find(t => t.Id == taskId);
            if (task != null)
            {
                task.AssignedUserId = userId;
            }
        }

        public void SetTaskPriority(int taskId, int priority)
        {
            var task = Tasks.Find(t => t.Id == taskId);
            if (task != null)
            {
                task.Priority = priority;
            }
        }

        public void SetTaskStatus(int taskId, string status)
        {
            var task = Tasks.Find(t => t.Id == taskId);
            if (task != null)
            {
                task.Status = status;
            }
        }
    }
}
