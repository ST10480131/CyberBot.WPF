using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CyberBot
{
    public class ActivityLogger
    {
        private static ActivityLogger instance;

        public static ActivityLogger Instance
        {
            get
            {
                if (instance == null)
                    instance = new ActivityLogger();

                return instance;
            }
        }

        private Database db = new Database();

        public List<string> Entries { get; } = new List<string>();

        public void Log(string description)
        {
            Entries.Add(description);

            using (SqlConnection con = db.GetConnection())
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO ActivityLog(ActivityType,Description) VALUES(@type,@desc)",
                    con);

                cmd.Parameters.AddWithValue("@type", "Chatbot");
                cmd.Parameters.AddWithValue("@desc", description);

                cmd.ExecuteNonQuery();
            }
        }

        public List<string> GetLastActivities(int count = 10)
        {
            List<string> logs = new List<string>();

            using (SqlConnection con = db.GetConnection())
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    @"SELECT TOP (@count) Description, ActivityDate
                      FROM ActivityLog
                      ORDER BY ActivityDate DESC",
                    con);

                cmd.Parameters.AddWithValue("@count", count);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    logs.Add(
                        $"{Convert.ToDateTime(reader["ActivityDate"]):g} - {reader["Description"]}");
                }
            }

            return logs;
        }
    }
}
