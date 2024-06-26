using System.Windows;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ProjectLibrary;
using System;

namespace ProjectDImaPPTwo
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Объект для управления проектами
        private ProjectManagement projectManagement = new ProjectManagement();
        // Строка подключения к базе данных
        private string lineConnection;
        // Текущий проект для добавления/редактирования
        private Project currentProject;
        // Текущая задача для добавления/редактирования
        private Task currentTask;

        public MainWindow()
        {
            InitializeComponent();
            // Установить строку подключения к базе данных
            Connect("DESKTOP-IGIV7GF\\SQLEXPRESS", "Proekt");
        }

        // Метод для установки строки подключения
        public void Connect(string servername, string dbname)
        {
            lineConnection = $"Data Source={servername};Initial Catalog={dbname};Integrated Security=True";
        }

        // Метод для загрузки проектов из базы данных
        private void LoadProjects()
        {
            projectManagement.Projects.Clear();
            using (SqlConnection connection = new SqlConnection(lineConnection))
            {
                connection.Open();
                string query = "SELECT Id, Name, Description, StartDate, EndDate, Budget FROM Projects";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    projectManagement.Projects.Add(new Project
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        StartDate = reader.GetDateTime(3),
                        EndDate = reader.GetDateTime(4),
                        Budget = reader.GetDecimal(5)
                    });
                }
            }
            ProjectList.ItemsSource = projectManagement.Projects;
        }

        // Метод для загрузки задач из базы данных
        private void LoadTasks()
        {
            projectManagement.Tasks.Clear();
            using (SqlConnection connection = new SqlConnection(lineConnection))
            {
                connection.Open();
                string query = "SELECT Id, ProjectId, Name, Description, AssignedUserId, Priority, Status FROM Tasks";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    projectManagement.Tasks.Add(new Task
                    {
                        Id = reader.GetInt32(0),
                        ProjectId = reader.GetInt32(1),
                        Name = reader.GetString(2),
                        Description = reader.GetString(3),
                        AssignedUserId = reader.GetInt32(4),
                        Priority = reader.GetInt32(5),
                        Status = reader.GetString(6)
                    });
                }
            }
            TaskList.ItemsSource = projectManagement.Tasks;
        }

        // Метод для обработки события нажатия кнопки "Login"
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginInput.Text;
            string password = PasswordInput.Password;

            using (SqlConnection connection = new SqlConnection(lineConnection))
            {
                connection.Open();
                string query = "SELECT Id, FirstName, LastName, MiddleName, RoleId FROM Users WHERE Login = @login AND Password = @password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    var user = new User
                    {
                        Id = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        MiddleName = reader.GetString(3),
                        RoleId = reader.GetInt32(4)
                    };

                    MessageBox.Show($"Login successful. Welcome, {user.FirstName}!");
                    LoadProjects();
                    LoadTasks();
                }
                else
                {
                    MessageBox.Show("Invalid login or password");
                }
            }
        }

        // Метод для обработки события нажатия кнопки "Add Project"
        private void AddProject_Click(object sender, RoutedEventArgs e)
        {
            currentProject = new Project();
            ShowProjectDialog();
        }

        // Метод для обработки события нажатия кнопки "Edit Project"
        private void EditProject_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectList.SelectedItem is Project selectedProject)
            {
                currentProject = selectedProject;
                ShowProjectDialog();
            }
        }

        // Метод для обработки события нажатия кнопки "Add Task"
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            currentTask = new Task();
            ShowTaskDialog();
        }

        // Метод для обработки события нажатия кнопки "Edit Task"
        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is Task selectedTask)
            {
                currentTask = selectedTask;
                ShowTaskDialog();
            }
        }

        // Метод для отображения диалога проекта
        private void ShowProjectDialog()
        {
            ProjectNameInput.Text = currentProject.Name;
            ProjectDescriptionInput.Text = currentProject.Description;
            ProjectStartDateInput.SelectedDate = currentProject.StartDate;
            ProjectEndDateInput.SelectedDate = currentProject.EndDate;
            ProjectBudgetInput.Text = currentProject.Budget.ToString();
            ProjectDialog.Visibility = Visibility.Visible;
        }

        // Метод для отображения диалога задачи
        private void ShowTaskDialog()
        {
            TaskNameInput.Text = currentTask.Name;
            TaskDescriptionInput.Text = currentTask.Description;
            TaskAssignedUserIdInput.Text = currentTask.AssignedUserId.ToString();
            TaskPriorityInput.Text = currentTask.Priority.ToString();
            TaskStatusInput.Text = currentTask.Status;
            TaskDialog.Visibility = Visibility.Visible;
        }

        // Метод для сохранения проекта
        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            currentProject.Name = ProjectNameInput.Text;
            currentProject.Description = ProjectDescriptionInput.Text;
            currentProject.StartDate = ProjectStartDateInput.SelectedDate ?? DateTime.Now;
            currentProject.EndDate = ProjectEndDateInput.SelectedDate ?? DateTime.Now;
            currentProject.Budget = decimal.TryParse(ProjectBudgetInput.Text, out decimal budget) ? budget : 0;

            using (SqlConnection connection = new SqlConnection(lineConnection))
            {
                connection.Open();
                if (currentProject.Id == 0)
                {
                    string query = "INSERT INTO Projects (Name, Description, StartDate, EndDate, Budget) VALUES (@name, @description, @startDate, @endDate, @budget)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", currentProject.Name);
                    command.Parameters.AddWithValue("@description", currentProject.Description);
                    command.Parameters.AddWithValue("@startDate", currentProject.StartDate);
                    command.Parameters.AddWithValue("@endDate", currentProject.EndDate);
                    command.Parameters.AddWithValue("@budget", currentProject.Budget);
                    command.ExecuteNonQuery();
                }
                else
                {
                    string query = "UPDATE Projects SET Name = @name, Description = @description, StartDate = @startDate, EndDate = @endDate, Budget = @budget WHERE Id = @id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", currentProject.Name);
                    command.Parameters.AddWithValue("@description", currentProject.Description);
                    command.Parameters.AddWithValue("@startDate", currentProject.StartDate);
                    command.Parameters.AddWithValue("@endDate", currentProject.EndDate);
                    command.Parameters.AddWithValue("@budget", currentProject.Budget);
                    command.Parameters.AddWithValue("@id", currentProject.Id);
                    command.ExecuteNonQuery();
                }
            }
            ProjectDialog.Visibility = Visibility.Hidden;
            LoadProjects();
        }

        // Метод для сохранения задачи
        private void SaveTask_Click(object sender, RoutedEventArgs e)
        {
            currentTask.Name = TaskNameInput.Text;
            currentTask.Description = TaskDescriptionInput.Text;
            currentTask.AssignedUserId = int.TryParse(TaskAssignedUserIdInput.Text, out int userId) ? userId : 0;
            currentTask.Priority = int.TryParse(TaskPriorityInput.Text, out int priority) ? priority : 0;
            currentTask.Status = TaskStatusInput.Text;

            using (SqlConnection connection = new SqlConnection(lineConnection))
            {
                connection.Open();
                if (currentTask.Id == 0)
                {
                    string query = "INSERT INTO Tasks (ProjectId, Name, Description, AssignedUserId, Priority, Status) VALUES (@projectId, @name, @description, @assignedUserId, @priority, @status)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@projectId", currentTask.ProjectId);
                    command.Parameters.AddWithValue("@name", currentTask.Name);
                    command.Parameters.AddWithValue("@description", currentTask.Description);
                    command.Parameters.AddWithValue("@assignedUserId", currentTask.AssignedUserId);
                    command.Parameters.AddWithValue("@priority", currentTask.Priority);
                    command.Parameters.AddWithValue("@status", currentTask.Status);
                    command.ExecuteNonQuery();
                }
                else
                {
                    string query = "UPDATE Tasks SET Name = @name, Description = @description, AssignedUserId = @assignedUserId, Priority = @priority, Status = @status WHERE Id = @id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", currentTask.Name);
                    command.Parameters.AddWithValue("@description", currentTask.Description);
                    command.Parameters.AddWithValue("@assignedUserId", currentTask.AssignedUserId);
                    command.Parameters.AddWithValue("@priority", currentTask.Priority);
                    command.Parameters.AddWithValue("@status", currentTask.Status);
                    command.Parameters.AddWithValue("@id", currentTask.Id);
                    command.ExecuteNonQuery();
                }
            }
            TaskDialog.Visibility = Visibility.Hidden;
            LoadTasks();
        }

        // Обработчик события GotFocus для LoginInput
        private void LoginInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (LoginInput.Text == "Enter Login")
            {
                LoginInput.Text = "";
                LoginInput.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        // Обработчик события LostFocus для LoginInput
        private void LoginInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginInput.Text))
            {
                LoginInput.Text = "Enter Login";
                LoginInput.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        // Обработчик события GotFocus для PasswordInput
        private void PasswordInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordInput.Password == "Enter Password")
            {
                PasswordInput.Password = "";
                PasswordInput.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        // Обработчик события LostFocus для PasswordInput
        private void PasswordInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordInput.Password))
            {
                PasswordInput.Password = "Enter Password";
                PasswordInput.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        // Обработчик события PasswordChanged для PasswordInput
        private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordInput.Password == "Enter Password")
            {
                PasswordInput.Password = "";
                PasswordInput.Foreground = System.Windows.Media.Brushes.Black;
            }
        }
    }
}
