using System;
using System.Media;

namespace CyberBot
{//start of namespace
    public class voice_greeting
    {//start of class

        //void method to play the sound named greet
        public void greet()
        { //start of greet method

            // the path with greeting.wav
            string filePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "greet.wav"
            );

            //create an instance for the soundPlayer class
            SoundPlayer greetMe = new SoundPlayer(auto_path);
            //then greet
            greetMe.Play();

        }//end of greet method

    }//end of class
}//end of namespace
