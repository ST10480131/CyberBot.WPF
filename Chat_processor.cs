using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CyberBot
{
    class Chat_processor
    {

        public class chat_processor
        {//start of class

            // References passed in from MainWindow
            private ArrayList reply;
            private ArrayList ignore;
            private string username;
            private int counting;
            private ListView chats;

            // last_topic stores the most recent cybersecurity keyword the user asked about
            // this enables conversation flow — follow-up phrases like "tell me more" re-use it
            private string last_topic;

            // sentiment words list — used to detect when a sentiment was matched so the bot
            // can automatically follow up with a cybersecurity tip on the same topic
            private static readonly List<string> sentiment_words = new List<string>
        {
            "frustrated", "confused", "worried", "happy", "sad", "angry", "curious"
        };

            // follow-up phrases — when the user types any of these the bot continues
            // the last topic without the user needing to repeat themselves
            private static readonly List<string> followup_phrases = new List<string>
        {
            "tell me more", "another tip", "give me another tip",
            "explain more", "more", "another", "continue", "go on", "keep going", "elaborate"
        };

            public chat_processor(ArrayList reply, ArrayList ignore, ListView chats)
            {
                this.reply = reply;
                this.ignore = ignore;
                this.chats = chats;
                this.counting = 0;
                this.username = string.Empty;

                // initialise last_topic to empty — no topic has been discussed yet
                this.last_topic = string.Empty;
            }

            // Allow MainWindow to update username after submit_name
            public void set_username(string name)
            {
                username = name;
            }

            // Allow MainWindow to read current counting value if needed
            public int get_counting()
            {
                return counting;
            }


            //start of ai_chat method
            public void ai_check(string questions)
            {

                // Check if user entered anything meaningful
                if (string.IsNullOrWhiteSpace(questions))
                {
                    error_method("Cypher", "Please enter a valid question.", true);
                    return;
                }

                // Check if the question contains only special characters or empty after cleaning
                if (questions.Length == 0 || string.IsNullOrWhiteSpace(questions))
                {
                    error_method("Cypher", "I couldn't understand that.", true);
                    return;
                }

                // --- CONVERSATION FLOW: check for follow-up phrases before anything else ---
                // if the user says something like "tell me more" or "another tip",
                // and we have a last_topic saved, re-run ai_check on that topic directly
                string lowered_input = questions.ToLower().Trim();
                if (!string.IsNullOrWhiteSpace(last_topic))
                {
                    foreach (string phrase in followup_phrases)
                    {
                        if (lowered_input.Contains(phrase))
                        {
                            // inform the user we are continuing the previous topic
                            error_method("Cypher", "Sure! Here is another tip on " + last_topic + ":");
                            // re-run ai_check using the saved topic so a new random response is picked
                            ai_check(last_topic);
                            return;
                        }
                    }
                }
                // --- END CONVERSATION FLOW CHECK ---


                // Variables for processing
                string[] words = questions.ToLower().Split(new char[] { ' ', ',', '.', '?', '!', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
                bool found = false;
                string message = string.Empty;
                Random indexer = new Random();
                List<string> per_word = new List<string>();
                List<string> answers_found = new List<string>();

                // matched_topic tracks the first meaningful keyword matched in this message
                // it is saved to last_topic at the end so follow-up phrases work next turn
                string matched_topic = string.Empty;

                // sentiment_triggered stores which sentiment word (if any) was matched
                // so the bot can auto-follow with a relevant cybersecurity tip
                string sentiment_triggered = string.Empty;


                // Process each word
                foreach (string word in words)
                {
                    // Skip very short words or ignored words
                    if (word.Length < 3 || ignore.Contains(word.ToLower()))
                        continue;

                    per_word.Clear();

                    //start of interests
                    if (word.Contains("interested"))
                    {
                        string store_interests = string.Empty;
                        bool found_interest = false;

                        HashSet<string> currentInterests = new HashSet<string>();

                        foreach (string interest in words)
                        {
                            // CLEAN INPUT
                            string clean = interest.ToLower().Trim();
                            clean = Regex.Replace(clean, @"[^a-zA-Z0-9\s]", "");

                            // FILTER NOISE WORDS
                            if (!ignore.Contains(clean) && clean != "interested" && clean != "and" && clean != "in" && clean.Length >= 3)
                            {
                                found_interest = true;
                                currentInterests.Add(clean);
                            }
                        }


                        // prepare interests
                        store_interests = string.Join(", ", currentInterests);

                        if (found_interest && !string.IsNullOrWhiteSpace(store_interests))
                        {
                            string filename = "interested_topic.txt";
                            bool userFound = false;

                            if (File.Exists(filename))
                            {
                                string[] lines = File.ReadAllLines(filename);

                                for (int i = 0; i < lines.Length; i++)
                                {
                                    if (lines[i].StartsWith(username))
                                    {
                                        userFound = true;

                                        //get all the interests
                                        string existing = lines[i]
                                            .Replace(username + " interested in:", "")
                                            .ToLower();

                                        HashSet<string> existingSet = new HashSet<string>(
                                            existing.Split(',').Select(x => x.Trim())
                                            .Where(x => x != "")
                                        );

                                        // remove duplicates
                                        foreach (string item in currentInterests)
                                        {
                                            existingSet.Add(item);
                                        }

                                        string finalList = string.Join(", ", existingSet);

                                        lines[i] = username + " interested in: " + finalList;
                                        File.WriteAllLines(filename, lines);

                                        message += "great, i added " + store_interests + " to your interests and ";
                                        break;
                                    }
                                }
                            }

                            if (!userFound)
                            {
                                File.AppendAllText(
                                    filename,
                                    username + " interested in: " + store_interests + "\n"
                                );

                                message += "great, i will remember that you are interested in " + store_interests + " and ";
                            }
                        }
                        else
                        {
                            message += "Please specify what you're interested in (e.g., 'I am interested in cybersecurity')";
                        }
                    }
                    //end of interests


                    // Search for matching answers
                    bool wordFound = false;
                    foreach (string answer in reply)
                    {
                        if (answer.ToLower().Contains(word))
                        {
                            wordFound = true;
                            per_word.Add(answer);
                        }
                    }

                    if (wordFound && per_word.Count > 0)
                    {
                        found = true;
                        int indexing = indexer.Next(0, per_word.Count);
                        answers_found.Add(per_word[indexing]);

                        // save the first matched keyword as the topic for this turn
                        // skip saving sentiment words — we only want cybersecurity topics saved
                        // so that follow-up phrases recall the actual subject, not the emotion
                        if (string.IsNullOrEmpty(matched_topic) && !sentiment_words.Contains(word))
                        {
                            matched_topic = word;
                        }

                        // if this word is a sentiment word, record it so we can auto-follow up
                        if (sentiment_words.Contains(word) && string.IsNullOrEmpty(sentiment_triggered))
                        {
                            sentiment_triggered = word;
                        }
                    }
                }

                // Show responses or error message
                if (found && answers_found.Count > 0)
                {
                    // Remove duplicate answers
                    answers_found = answers_found.Distinct().ToList();

                    foreach (string per_answer in answers_found)
                    {
                        message += per_answer + "\n";
                    }

                    error_method("Cypher", message.TrimEnd('\n'));

                    chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);

                    // --- SENTIMENT AUTO-TIP ---
                    // if a sentiment was detected (e.g. "worried"), automatically share a
                    // cybersecurity tip related to the topic in the same message without
                    // making the user type again — requirement: bot must continue the topic
                    if (!string.IsNullOrEmpty(sentiment_triggered))
                    {
                        // look for any cybersecurity keyword also present in the user's message
                        // so the tip is relevant to what they mentioned (e.g. "worried about scams")
                        string auto_topic = string.Empty;
                        foreach (string word in words)
                        {
                            if (!sentiment_words.Contains(word) && !ignore.Contains(word) && word.Length >= 3)
                            {
                                // check if this word has matching answers in the reply list
                                foreach (string answer in reply)
                                {
                                    if (answer.ToLower().Contains(word))
                                    {
                                        auto_topic = word;
                                        break;
                                    }
                                }
                                if (!string.IsNullOrEmpty(auto_topic)) break;
                            }
                        }

                        // if no specific topic was found alongside the sentiment,
                        // fall back to a general cybersecurity tip
                        if (string.IsNullOrEmpty(auto_topic))
                        {
                            auto_topic = "cybersecurity";
                        }

                        // deliver the tip immediately — user does not need to ask again
                        error_method("Cypher", "Here is a tip to help:");
                        ai_check(auto_topic);
                    }
                    // --- END SENTIMENT AUTO-TIP ---

                    // update last_topic so follow-up phrases work on the next turn
                    if (!string.IsNullOrEmpty(matched_topic))
                    {
                        last_topic = matched_topic;
                    }
                }
                else
                {
                    // when nothing is found
                    string[] fallbackMessages = {
                "I'm sorry, I don't understand that. Could you rephrase your question?",
                "I didn't quite get that. Try asking about cyber security topics.",
                "Hmm, I'm not sure how to respond to that. Can you ask something else?",
                "I couldn't find an answer for that. Please ask about programming, security, or technology.",
                "My apologies, I don't have information on that topic yet."
                };

                    Random random = new Random();
                    string fallbackMessage = fallbackMessages[random.Next(fallbackMessages.Length)];
                    error_method("Cypher", fallbackMessage, true);
                }

            }
            //end of ai_chat method


            //method to remove special characters
            public string RemoveSpecialCharacters(string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                    return string.Empty;

                StringBuilder sanitized = new StringBuilder();

                foreach (char c in input)
                {
                    // Keep letters, numbers, spaces, and basic punctuation
                    if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '\'' || c == '-')
                    {
                        sanitized.Append(c);
                    }
                    else
                    {
                        // Replace other special characters with space
                        sanitized.Append(' ');
                    }
                }

                // Clean up extra spaces and trim
                string result = sanitized.ToString();
                result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+", " ").Trim();

                return result;
            }
            //end of method to remove special characters


            //method count to show interests randomly
            public void auto_show_interest()
            {
                //check if three times
                if (counting == 3)
                {
                    //read the user's interests from file
                    string filename = "interested_topic.txt";

                    if (File.Exists(filename))
                    {
                        string[] lines = File.ReadAllLines(filename);

                        //find the user's line
                        foreach (string line in lines)
                        {
                            if (line.StartsWith(username))
                            {
                                //get the interests part
                                int colonIndex = line.IndexOf("interested in:");
                                if (colonIndex >= 0)
                                {
                                    string interests = line.Substring(colonIndex + 14).Trim();

                                    //show reminder of interests
                                    error_method("Cypher", "Just a reminder, you are interested in " + interests + " and ");
                                    ai_check(interests);
                                    break;
                                }
                            }
                        }
                    }

                    //reset counting
                    counting = 0;
                }
                else
                {
                    //incrementing
                    counting += 1;
                }
            }
            //end of count interest method


            // error method — normal messages (isError defaults to false)
            public void error_method(string name, string message)
            {
                error_method(name, message, false);
            }//end of error method overload

            // error method with isError flag — set isError to true for error/fallback messages
            public void error_method(string name, string message, bool isError)
            {
                // Create a border for chats
                Border messageBorder = new Border
                {
                    Margin = new Thickness(0, 2, 0, 2),
                    Padding = new Thickness(5, 3, 5, 3),
                    CornerRadius = new CornerRadius(5)
                };

                // Set different background for user vs bot
                if (name.ToLower().Contains("cypher"))
                {// DeepSky blue
                    messageBorder.Background = new SolidColorBrush(Color.FromRgb(54, 57, 63));
                    messageBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(114, 137, 218));
                }
                else
                {    // sea green
                    messageBorder.Background = new SolidColorBrush(Color.FromRgb(47, 49, 54));
                    messageBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(67, 181, 129));
                }
                messageBorder.BorderThickness = new Thickness(1);

                TextBlock messageText = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(2)
                };

                // Cypher name = DeepSky blue, username = sea green
                Brush nameColor = name.ToLower().Contains("cypher") ?
                                  Brushes.DeepSkyBlue : Brushes.SeaGreen;

                // Error messages = Red, normal messages = Black
                Brush messageColor = isError ? Brushes.Red : Brushes.LimeGreen;

                messageText.Inlines.Add(new Run
                {
                    Text = name + ": ",
                    Foreground = nameColor,
                    FontWeight = FontWeights.Bold
                });

                messageText.Inlines.Add(new Run
                {
                    Text = message,
                    Foreground = messageColor
                });

                messageBorder.Child = messageText;
                chats.Items.Add(messageBorder);

                chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);
            }//end of error method

        }
    }
    }//end of namespace
