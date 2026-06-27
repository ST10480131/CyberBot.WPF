using System;
using System.Collections.Generic;
using System.Text;

namespace CyberBot
{
    class Quiz
    {
        private int score = 0;
        private int index = 0;

        public List<Question> Questions = new List<Question>()
        {
            
            new Question("Phishing is a cyber attack.", new string[] { "True", "False" }, 0),
            new Question("You should share your password with friends.", new string[] { "True", "False" }, 1),
            new Question("2FA improves account security.", new string[] { "True", "False" }, 0),
            new Question("Malware is harmless software.", new string[] { "True", "False" }, 1),
            new Question("Public WiFi is always safe.", new string[] { "True", "False" }, 1),

            
            new Question("What is the strongest password?",
                new string[] { "123456", "password", "P@ssw0rd!2026", "admin123" }, 2),

            new Question("What does phishing try to steal?",
                new string[] { "Photos", "Personal data", "Battery life", "Internet speed" }, 1),

            new Question("What should you do with suspicious emails?",
                new string[] { "Click links", "Ignore/delete", "Reply immediately", "Forward to strangers" }, 1),

            new Question("Which protects against malware?",
                new string[] { "Antivirus", "Brightness settings", "Keyboard", "Browser history" }, 0),

            new Question("What is safe browsing behavior?",
                new string[] { "Download unknown files", "Click random links", "Verify websites", "Share OTP codes" }, 2)
        };

        
        public Question GetQuestion()
        {
            if (index < Questions.Count)
                return Questions[index];

            return null;
        }

       
        public bool Answer(int selectedIndex)
        {
            bool correct = Questions[index].CorrectIndex == selectedIndex;

            if (correct)
                score++;

            index++;

            return correct;
        }

      
        public int GetScore()
        {
            return score;
        }

       
        public bool IsFinished()
        {
            return index >= Questions.Count;
        }

      
        public void Reset()
        {
            score = 0;
            index = 0;
        }
    }

    
    public class Question
    {
        public string Text { get; set; }
        public string[] Options { get; set; }
        public int CorrectIndex { get; set; }

        public Question(string text, string[] options, int correctIndex)
        {
            Text = text;
            Options = options;
            CorrectIndex = correctIndex;
        }
    }
}
