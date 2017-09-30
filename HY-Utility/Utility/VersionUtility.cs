using HY_Utility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Xml;

namespace HY_Utility
{
    class VersionUtility
    {
        private WebClient webClient = new WebClient();

        public static void CheckVersion()
        {
            ModData[] modDataList = { ModData.ChaosGreedier };

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
