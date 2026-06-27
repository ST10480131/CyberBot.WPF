using System;
using System.Linq;

namespace CyberBot
{
    public class CommandProcessor
    {
      
        public bool IsQuizCommand(string input)
        {
            string msg = input.ToLower();

            return msg.Contains("quiz") ||
                   msg.Contains("test my knowledge") ||
                   msg.Contains("let's try a quiz") ||
                   msg.Contains("start quiz") ||
                   msg.Contains("knowledge test");
        }

        
        public bool IsActivityCommand(string input)
        {
            string msg = input.ToLower();

            return msg.Contains("activity log") ||
                   msg.Contains("what have you done") ||
                   msg.Contains("show activity") ||
                   msg.Contains("recent actions") ||
                   msg.Contains("log history");
        }

     
        // NLP KEYWORDS
        
        public string DetectKeyword(string input)
        {
            string msg = input.ToLower();

            if (msg.Contains("password")) return "password";
            if (msg.Contains("phishing")) return "phishing";
            if (msg.Contains("task")) return "task";
            if (msg.Contains("reminder")) return "reminder";

            return "general";
        }
        public bool IsTaskCommand(string input)
        {
            input = input.ToLower();

            return input.Contains("add task") ||
                   input.Contains("create task") ||
                   input.Contains("new task") ||
                   input.Contains("update task") ||
                   input.Contains("delete task") ||
                   input.Contains("complete task") ||
                   input.Contains("mark task");
        }
    }
}
