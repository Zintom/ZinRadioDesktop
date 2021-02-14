using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZinRadioDesktop
{
    static class Program
    {
        public const string WebsiteHost = "http://www.zinstorm.com";

        public const string StationListUrl = "https://raw.githubusercontent.com/Zintom/ZinRadioDesktop/master/Database/stations_v2.txt";
        public const string StationListVersionUrl = "https://raw.githubusercontent.com/Zintom/ZinRadioDesktop/master/Database/stations_v2_version.txt";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        public static double CosineInterpolate(
        double y1, double y2,
        double mu)
        {
            double mu2;

            mu2 = (1 - Math.Cos(mu * Math.PI)) / 2;
            return (y1 * (1 - mu2) + y2 * mu2);
        }
    }
}
