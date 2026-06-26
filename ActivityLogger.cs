using System;
using System.Collections.ObjectModel;
using System.IO;

namespace CyberBot;

public class ActivityLogger
{
    public static ActivityLogger Instance { get; } = new();
    public ObservableCollection<string> Entries { get; } = new();

    private readonly string Logfile = Path.Combine(AppContext.BaseDirectory, "activity.log");

    private ActivityLogger()
    {
        if (File.Exists(Logfile))
        {
            foreach (var line in File.ReadAllLines(Logfile))
            {
                Entries.Add(line);
            }

        }
    }

    public void Log(string action)
    {
        var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {action}";
        Entries.Add(line);
        try { File.AppendAllText(Logfile, line + Environment.NewLine); } catch { }
    }

    public void Clear()
    {
        Entries.Clear();
        try { File.Delete(Logfile); } catch { }
    }

}
