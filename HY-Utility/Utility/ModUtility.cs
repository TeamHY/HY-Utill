using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HY_Utility
{
    class ModUtility
    {
        public static void DisabledMod(string modDirName)
        {
            var modPath = MainWindow.ModsPath + String.Format(@"\{0}", modDirName);

            if (Directory.Exists(modPath))
            {
                var fileInfo = new FileInfo(modPath + @"\disable.it");

                if (!fileInfo.Exists)
                {
                    var fileStream = fileInfo.Create();
                    fileStream.Close();
                }
            }
        }

        public static void DisabledAllMods()
        {
            var directoryInfo = new DirectoryInfo(MainWindow.ModsPath);

            if (directoryInfo.Exists)
            {
                var subDirectoryInfo = directoryInfo.GetDirectories("*", SearchOption.AllDirectories);

                foreach (var directory in subDirectoryInfo)
                    DisabledMod(directory.Name);
            }
        }

        public static void RemoveMod(string modDirName)
        {
            if (!String.IsNullOrEmpty(modDirName))
                if (Directory.Exists(MainWindow.ModsPath + String.Format(@"\{0}", modDirName)))
                    DirectoryForceDelete(MainWindow.ModsPath + String.Format(@"\{0}", modDirName));
        }

        public static void DirectoryForceDelete(string srcPath)
        {
            try
            {
                var directoryInfo = new DirectoryInfo(srcPath);

                var files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);

                foreach (FileInfo file in files)
                    file.Attributes = FileAttributes.Normal;

                Directory.Delete(srcPath, true);
            }
            catch (Exception e)
            {
                return;
            }
        }
    }
}
