using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HY_Utility.Data
{
    class ModData
    {
        public static ModData ChaosGreedier = new ModData("chaosgreedier");

        public string Name { get; private set; }
        public string Directory { get { return MainWindow.ModsPath + String.Format(@"\{0}", Name); } }

        public string CurrentVersion { get; set; }
        public string LatestVersion { get; set; }
        public string LatestUrl { get; set; }
        public string LatestChangelog { get; set; }

        public ModData(string name)
        {
            Name = name;
        }
    }
}
