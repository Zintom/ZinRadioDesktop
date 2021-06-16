using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ZinRadioDesktop.Plugins
{
    /// <summary>
    /// The plugin interface for displaying visualisations that represent the currently playing audio.
    /// </summary>
    public interface IAudioVisualizer
    {
        /// <returns>The information regarding this plugin.</returns>
        PluginInfo Load();

        /// <summary>
        /// Draws a frame which is a visualization of the given <paramref name="samples"/>.
        /// </summary>
        /// <param name="samples"></param>
        /// <returns>A <see cref="Bitmap"/> which represents the <paramref name="samples"/> in some visual form.</returns>
        Bitmap DrawFrame(byte[] samples, int canvasWidth, int canvasHeight);
    }

    public static class VisualizerPluginLoader
    {
        private static readonly string _pluginDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\Plugins";

        /// <summary>
        /// Gets all available <see cref="IAudioVisualizer"/>'s available in the <b>Plugins</b> directory.
        /// </summary>
        public static IReadOnlyList<IAudioVisualizer> GetAvailableAudioVisualizers()
        {
            List<IAudioVisualizer> plugins = new();

            foreach (string file in Directory.GetFiles(_pluginDirectory))
            {
                IAudioVisualizer? plugin = PluginBase.CreateTypeFromDll<IAudioVisualizer>(file);
                if (plugin != null)
                {
                    plugins.Add(plugin);
                }
            }

            return plugins;
        }
    }
}
