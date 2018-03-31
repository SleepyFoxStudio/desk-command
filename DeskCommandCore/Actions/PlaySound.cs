using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace DeskCommandCore.Actions
{
    class PlaySound : InterfaceAction
    {
        public PlaySound(string soundFile)
        {
            SoundFile = soundFile;
        }
        
        public string SoundFile { get; set; }

        public void Do()
        {
            if (string.IsNullOrWhiteSpace(SoundFile)) return;
            var path = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(path, "wwwRoot", "action", "playsound", SoundFile);
            if (File.Exists(filePath))
            {
                using (var audioFile = new AudioFileReader(filePath))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(500);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Error finding sound to play '{filePath}'");
            }
        }
    }
}
