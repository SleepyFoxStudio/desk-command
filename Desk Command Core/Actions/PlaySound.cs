using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Desk_Command_Core.Actions
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

                var wplayer = new WMPLib.WindowsMediaPlayer {URL = filePath };
                wplayer.controls.play();
            }
            else
            {
                Console.WriteLine($"Error finding sound to play '{filePath}'");
            }
        }
    }
}
