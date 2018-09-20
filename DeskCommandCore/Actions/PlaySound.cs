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

        public Task Do()
        {
            if (string.IsNullOrWhiteSpace(SoundFile)) return Task.CompletedTask;
            var path = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(path, "wwwRoot", "action", "playsound", SoundFile);
            if (File.Exists(filePath))
            {
                return PlayAsync(filePath);
            }
            else
            {
                Console.WriteLine($"Error finding sound to play '{filePath}'");
            }
            return Task.CompletedTask;
        }

        private async Task PlayAsync(string filePath)
        {
            var tcs = new TaskCompletionSource<StoppedEventArgs>();

            EventHandler<StoppedEventArgs> stopHandler = (sender, args) =>
            {
                tcs.SetResult(args);
            };

            using (var audioFile = new AudioFileReader(filePath))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                outputDevice.PlaybackStopped += stopHandler;
                await tcs.Task;
                outputDevice.PlaybackStopped -= stopHandler;
            }
        }
    }
}
