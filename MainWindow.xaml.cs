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

        public MainWindow()
        {
            InitializeComponent();

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


        //send event handler
        private void send(object sender, RoutedEventArgs e)
        {
            // Get the question from the design and sanitize it
            string rawQuestion = question.Text.ToString().Trim();

            if (string.IsNullOrWhiteSpace(rawQuestion))
            {
                processor.error_method("Cypher", "Please enter a question.", true);
                return;
            }

            // Remove special characters and clean the question
            string questions = processor.RemoveSpecialCharacters(rawQuestion);

            // Show what the user typed 
            processor.error_method(username, rawQuestion);

            // ai chats and auto_show_interest
            processor.auto_show_interest();
            processor.ai_check(questions);

            // Clear the input box
            question.Clear();
        }
        //end for the username submit


    }//end of class
}//end of namespace