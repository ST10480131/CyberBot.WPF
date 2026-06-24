using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CyberBot
{//start of namespace
    public class user_name
    {//start of user_name class

        public string submit_name(TextBox user_name_input, ListView chats, TextBlock error_label)
        {//start of submit_name

            //temp variables 
            string filename = "user_names.txt";

            //check if the filename exists or not , then auto create
            if (!File.Exists(filename))
            {
                //auto create the file using AppendAllText() function
                File.AppendAllText(filename, "auto_create\n");

            }//end 

            //temp variables
            string name = user_name_input.Text.ToString().Trim();

            // validate — if empty show the error label and stop
            if (string.IsNullOrWhiteSpace(name))
            {
                error_label.Visibility = Visibility.Visible;
                return string.Empty;
            }

            // hide the error label if name is valid
            error_label.Visibility = Visibility.Hidden;

            bool found = check_name(name);

            //check if the user is found or not and write the name in a text file
            if (!found)
            {//start of if
                //write the name in a text file
                File.AppendAllText(filename, name + "\n");
                //then welcome the user with typing effect
                type_message("Cypher", "Hey " + name + " welcome to AI cybersecurity", chats, false);

            }//end of if
            else
            {//start of else

                //welcome the user back with typing effect
                type_message("Cypher", "Hey " + name + " welcome back, how can i help you today", chats, false);

            }//end of else

            //return name
            return name;

        }//end of submit_name


        //helper method for word-by-word typing effect displayed in the chat ListView
        private void type_message(string sender, string message, ListView chats, bool isError)
        {
            // Build the border and textblock first (empty message)
            Border messageBorder = new Border
            {
                Margin = new Thickness(0, 2, 0, 2),
                Padding = new Thickness(5, 3, 5, 3),
                CornerRadius = new CornerRadius(5)
            };

            if (sender.ToLower().Contains("cypher"))
            {
                messageBorder.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255));
                messageBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(173, 216, 230));
            }
            else
            {
                messageBorder.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                messageBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(211, 211, 211));
            }
            messageBorder.BorderThickness = new Thickness(1);

            TextBlock messageText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(2)
            };

            // Cypher name = RoyalBlue, username = LimeGreen
            Brush nameColor = sender.ToLower().Contains("cypher") ?
                              Brushes.RoyalBlue : Brushes.LimeGreen;

            // Error messages = Red, normal messages = Black
            Brush messageColor = isError ? Brushes.Red : Brushes.Black;

            Run nameRun = new Run
            {
                Text = sender + ": ",
                Foreground = nameColor,
                FontWeight = FontWeights.Bold
            };

            Run messageRun = new Run
            {
                Text = string.Empty,
                Foreground = messageColor
            };

            messageText.Inlines.Add(nameRun);
            messageText.Inlines.Add(messageRun);
            messageBorder.Child = messageText;
            chats.Items.Add(messageBorder);

            // type word by word with 150ms delay
            string[] words = message.Split(' ');
            foreach (string word in words)
            {
                messageRun.Text += word + " ";
                chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);
                chats.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() => { }));
                Thread.Sleep(150);
            }

        }//end of type_message


        //method to check name of the user
        private Boolean check_name(string name)
        {//start of check method

            //temp variable
            string filename = "user_names.txt";

            bool found_name = false;

            //store or get all the names in the text file and store in an 1D array
            string[] names = File.ReadAllLines(filename);

            //foreach to search the name of the user
            foreach (string name_found in names)
            { //start of loop

                //if statement to check for the username
                if (name_found.ToLower() == name.ToLower())
                {//start if

                    //found_name set to true
                    found_name = true;

                }//end of if

            }//end of the loop

            //return the status of found or not [ true or false ]
            return found_name;

        }//end check method

    }//end of user_name class
}//end of namespace