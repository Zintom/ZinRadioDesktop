using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zin_Radio
{
    public static class WebStreamPlayer
    {
        public static WMPLib.WindowsMediaPlayer WebAudioPlayer = new WMPLib.WindowsMediaPlayer();

        // Constructor
        static WebStreamPlayer()
        {

        }

        /// <summary>
        /// Plays the MP3 Stream.
        /// </summary>
        public static void PlayStream(string url)
        {
            WebAudioPlayer.controls.stop();
            WebAudioPlayer.URL = url;
            WebAudioPlayer.controls.play();
        }

    }
}