using CyberBot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CyberBot.Chat_processor;

namespace CyberBot
{//start of namespace

    public partial class MainWindow : Window
    {//start of class


        //creating an instance for the class Array
        ArrayList reply = new ArrayList();
        ArrayList ignore = new ArrayList();
        user_name check_name = new user_name();

        //variables
        string username = string.Empty;
        string pre_question = string.Empty;
        int counting = 0;

        //chat_processor instance
        chat_processor processor;
        private bool activityVisible = false;

        private TaskManager taskManager = new TaskManager();
        private Quiz quiz = new Quiz();
        private CommandProcessor cmd = new CommandProcessor();
        private bool quizActive = false;
        private System.Windows.Threading.DispatcherTimer reminderTimer;
        private SmartTaskParser taskParser = new SmartTaskParser();
        

        public MainWindow()
        {
            InitializeComponent();
            UpdateDashboard();
            lstActivityLog.ItemsSource = ActivityLogger.Instance.Entries;

            new respond(reply, ignore) { };

            //create the chat_processor instance
            processor = new chat_processor(reply, ignore, chats);
         
         //creating an instance for the class voice_greeting 
         //with an object name greet
         voice_greeting greet = new voice_greeting();

         //call the voice method
         greet.greet();


        } 

      


        //proceed event handler
        private void proceed(object sender, RoutedEventArgs e)
        {
            //Hide home page grid and set Username grid visible
            home_grid.Visibility = Visibility.Hidden;
            username_grid.Visibility = Visibility.Visible;
        }



        //submit name  event handler
        private void submit_name(object sender, RoutedEventArgs e)
        {
            // pass the error label so user_name can show/hide it
            username = check_name.submit_name(usernames_input, chats, username_error_label);

            // if empty the user left the field blank — stay on username page
            if (string.IsNullOrWhiteSpace(username))
                return;

            // pass username to the processor
            processor.set_username(username);

            // Hide username page grid and set chats grid visible
            username_grid.Visibility = Visibility.Hidden;
            chat_grid.Visibility = Visibility.Visible;
        }


        private void send(object sender, RoutedEventArgs e)
        {
            string rawQuestion = question.Text.Trim();

            if (string.IsNullOrWhiteSpace(rawQuestion))
            {
                processor.error_method("Cypher", "Please enter a question.", true);
                return;
            }

            // Clean input
            string clean = processor.RemoveSpecialCharacters(rawQuestion);
            string lower = clean.ToLower();

            // Show user message
            processor.error_method(username, rawQuestion);

            
            // 1. TASK SYSTEM 
            
            if (cmd.IsTaskCommand(lower))
            {
                // ADD TASK
                if (lower.Contains("add") || lower.Contains("create") || lower.Contains("set"))
                {
                    string title = taskParser.ExtractTitle(rawQuestion);
                    DateTime date = taskParser.ExtractDateTime(rawQuestion);

                    taskManager.AddTask(title, "Cybersecurity task", date);

                    chats.Items.Add($"Bot: Task added → {title}");
                    chats.Items.Add($"Due: {date:yyyy-MM-dd HH:mm}");

                    ActivityLogger.Instance.Log($"Smart task created: {title}");

                    question.Clear();
                    return;
                }

                // DELETE TASK
                if (lower.Contains("delete"))
                {
                    int id = ExtractNumber(lower);

                    if (id > 0)
                    {
                        taskManager.DeleteTask(id);
                        chats.Items.Add($"Bot: Task {id} deleted.");
                        ActivityLogger.Instance.Log($"Task {id} deleted");
                    }
                    else
                    {
                        chats.Items.Add("Bot: Please specify a valid task ID.");
                    }

                    question.Clear();
                    return;
                }

                // COMPLETE TASK
                if (lower.Contains("complete") || lower.Contains("mark"))
                {
                    int id = ExtractNumber(lower);

                    if (id > 0)
                    {
                        taskManager.CompleteTask(id);
                        chats.Items.Add($"Bot: Task {id} marked as completed.");
                        ActivityLogger.Instance.Log($"Task {id} completed");
                    }
                    else
                    {
                        chats.Items.Add("Bot: Please specify a valid task ID.");
                    }

                    question.Clear();
                    return;
                }

                // SHOW TASKS
                chats.Items.Add("Bot: Current Tasks:");

                foreach (var t in taskManager.GetTasks())
                {
                    chats.Items.Add(
                        $"{t.TaskId}. {t.Title} | Due: {t.ReminderDate:yyyy-MM-dd} | Done: {t.IsCompleted}"
                    );
                }

                question.Clear();
                return;
            }

            
            // 2. QUIZ COMMAND
            
            if (cmd.IsQuizCommand(lower))
            {
                quiz.Reset();
                quizActive = true;

                chats.Items.Add("Bot: Quiz started! Answer with 1-4.");

                ShowQuestion(quiz.GetQuestion());

                ActivityLogger.Instance.Log("Quiz started");

                question.Clear();
                return;
            }

            
            // 3. ACTIVITY LOG COMMAND
            
            if (cmd.IsActivityCommand(lower))
            {
                chats.Items.Add("Bot: Last 10 activities:");

                foreach (var item in ActivityLogger.Instance.Entries.TakeLast(10))
                {
                    chats.Items.Add(item);
                }

                ActivityLogger.Instance.Log("Activity log viewed");

                question.Clear();
                return;
            }

           
            // 4. QUIZ ANSWERS MODE
            
            if (quizActive)
            {
                if (int.TryParse(clean, out int answer))
                {
                    bool correct = quiz.Answer(answer - 1);
                    chats.Items.Add(correct ? "Bot: Correct!" : "Bot: Wrong!");
                }

                if (quiz.IsFinished())
                {
                    quizActive = false;

                    chats.Items.Add($"Bot: Final Score: {quiz.GetScore()}/10");

                    ActivityLogger.Instance.Log("Quiz completed");
                }
                else
                {
                    ShowQuestion(quiz.GetQuestion());
                }

                question.Clear();
                return;
            }

            
            
            processor.auto_show_interest();
            processor.ai_check(clean);

            question.Clear();
        }

        private void ShowQuestion(Question q)
        {
            if (q == null) return;

            chats.Items.Add("Question: " + q.Text);

            for (int i = 0; i < q.Options.Length; i++)
            {
                chats.Items.Add($"{i + 1}. {q.Options[i]}");
            }
        }

        private void CheckReminders(object sender, EventArgs e)
        {
            var tasks = taskManager.GetTasks();

            foreach (var task in tasks)
            {
                if (!task.IsCompleted && task.ReminderDate <= DateTime.Now)
                {
                    ShowReminder(task);
                }
            }
        }

        private void ShowReminder(Task task)
        {
            MessageBox.Show(
                $"Reminder: {task.Title}\nDue: {task.ReminderDate}",
                "CyberBot Reminder",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            ActivityLogger.Instance.Log($"Reminder triggered for task {task.TaskId}");

            // prevent repeated popups
            task.ReminderDate = DateTime.MaxValue;
        }

        private void UpdateDashboard()
        {
            txtPendingTasks.Text =
                taskManager.GetTasks().Count(t => !t.IsCompleted).ToString();

            txtCompletedTasks.Text =
                taskManager.GetTasks().Count(t => t.IsCompleted).ToString();
        }

        private int ExtractNumber(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return -1;

            string number = "";

            foreach (char c in text)
            {
                if (char.IsDigit(c))
                {
                    number += c;
                }
            }

            if (int.TryParse(number, out int result))
                return result;

            return -1;
        }



    }//end of class
}//end of namespace
