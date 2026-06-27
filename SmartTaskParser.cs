using System;
using System.Text.RegularExpressions;

namespace CyberBot
{
    public class SmartTaskParser
    {
        // Extract task title
        public string ExtractTitle(string input)
        {
            input = input.ToLower();

            input = input.Replace("add task", "")
                         .Replace("create task", "")
                         .Replace("set task", "")
                         .Trim();

            // remove time phrases
            input = Regex.Replace(input, @"\b(tomorrow|today|next week|next month|in \d+ days?|at \d{1,2}(:\d{2})?\s*(am|pm)?)\b", "", RegexOptions.IgnoreCase);

            return string.IsNullOrWhiteSpace(input) ? "Cybersecurity Task" : input.Trim();
        }

        // Extract date/time
        public DateTime ExtractDateTime(string input)
        {
            input = input.ToLower();

            DateTime now = DateTime.Now;

            if (input.Contains("tomorrow"))
                return now.AddDays(1);

            if (input.Contains("next week"))
                return now.AddDays(7);

            if (input.Contains("next month"))
                return now.AddMonths(1);

            Match match = Regex.Match(input, @"in (\d+) days?");
            if (match.Success)
            {
                int days = int.Parse(match.Groups[1].Value);
                return now.AddDays(days);
            }

            // default fallback
            return now.AddDays(1);
        }
    }
}
