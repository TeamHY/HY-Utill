using KimUtility.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml;

namespace KimUtility
{
    class VersionUtility
    {
        private static WebClient webClient = new WebClient();

        public static void CheckProgramVersion()
        {
            try
            {
                string latestVersion = null;
                string latestUrl = null;

                var xmlDocument = new XmlDocument();
                xmlDocument.Load(MainWindow.storageUrl + "versiondata.xml");

                var xmlNodeList = xmlDocument.SelectNodes(String.Format("/versiondata/{0}", "utility"));

                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    latestVersion = xmlNode["version"].InnerText;
                    latestUrl = xmlNode["url"].InnerText;
                }

                if (!Assembly.GetExecutingAssembly().GetName().Version.ToString().Equals(latestVersion))
                {
                    MessageBoxResult result = MessageBox.Show("신 버전이 있습니다.\r\n업데이트 하시겠습니까?", "KimUtility", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            var updaterUrl = String.Format(@"{0}{1}/", latestUrl, latestVersion);
                            var updaterFileName = "KimUpdater.exe";

                            if (!Directory.Exists(MainWindow.TempFolderPath))
                                Directory.CreateDirectory(MainWindow.TempFolderPath);

                            var updaterFilePath = MainWindow.TempFolderPath + updaterFileName;

                            webClient.DownloadFile(new Uri(updaterUrl + updaterFileName), updaterFilePath);

                            Process.Start(updaterFilePath, String.Format("\"{0}\" \"{1}\" \"{2}\"", updaterUrl, Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().ProcessName));
                        }
                        catch
                        {
                            MessageBox.Show("업데이트 실패!", "KimUtility", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch
            {
                return;
            }
        }

        public static void CheckVersion()
        {
            ModData[] modDataList = { ModData.ChaosGreedier };

            CheckProgramVersion();

            CheckCurrentVersion(modDataList);
            CheckLatestVersion(modDataList);
        }

        private static void CheckCurrentVersion(ModData[] modDataList)
        {
            foreach (ModData modData in modDataList)
            {
                try
                {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(modData.Directory + @"\metadata.xml");

                    var xmlNodeList = xmlDocument.SelectNodes("/metadata");

                    foreach (XmlNode xmlNode in xmlNodeList)
                    {
                        modData.CurrentVersion = xmlNode["version"].InnerText;
                    }
                }
                catch
                {
                    modData.CurrentVersion = null;
                }
            }
        }

        private static void CheckLatestVersion(ModData[] modDataList)
        {
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(MainWindow.storageUrl + "versiondata.xml");

                foreach (ModData modData in modDataList)
                {
                    try
                    {
                        var xmlNodeList = xmlDocument.SelectNodes(String.Format("/versiondata/{0}", modData.Name));

                        foreach (XmlNode xmlNode in xmlNodeList)
                        {
                            modData.LatestVersion = xmlNode["version"].InnerText;
                            modData.LatestUrl = xmlNode["url"].InnerText;
                            modData.LatestChangelog = xmlNode["changelog"].InnerText;
                        }
                    }
                    catch
                    {
                        modData.LatestVersion = null;
                    }
                }
            }
            catch
            {
                foreach (ModData modData in modDataList)
                {
                    modData.LatestVersion = null;
                }

                MessageBox.Show("최신 버전을 불러오지 못했습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
