using System;
using System.IO;
using System.Media;
using System.Windows;

namespace CyberBot
{
    public class voice_greeting
    {
        public void greet()
        {
            string auto_path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "greet.wav"
            );

            if (File.Exists(auto_path))
            {
                SoundPlayer greetMe = new SoundPlayer(auto_path);
                greetMe.Play();
            }
            else
            {
                MessageBox.Show(
                    "Could not find greet.wav\n\n" +
                    auto_path
                );
            }
        }
    }
}
