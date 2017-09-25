using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace HY_Utill
{
    class VersionUtility
    {
        public static string LatestVersion { get; set; }
        public static string CurrentVersion { get; set; }

        private static WebClient webClient = new WebClient();

        public static void CheckVersion()
        {
            try
            {
                var currentXmlPath = MainWindow.ModsPath + @"\chaosgreedier\metadata.xml";

                var currentXml = new XmlDocument();

                currentXml.Load(currentXmlPath);

                var currentXmlNodeList = currentXml.SelectNodes("/metadata");

                foreach (XmlNode xn in currentXmlNodeList)
                {
                    CurrentVersion = xn["version"].InnerText;
                }
            }
            catch (Exception e)
            {
                CurrentVersion = null;
            }

            try
            {
                var latestXmlPath = System.IO.Path.GetTempFileName();

                var latestXml = new XmlDocument();

                webClient.DownloadFile(MainWindow.storageUrl + "versiondata.xml", latestXmlPath);
                latestXml.Load(latestXmlPath);

                var latestXmlNodeList = latestXml.SelectNodes("/versiondata/chaosgreedier");

                foreach (XmlNode xn in latestXmlNodeList)
                {
                    //Changelog = xn["changelog"].InnerText;

                    LatestVersion = xn["version"].InnerText;
                }
            }
            catch (Exception e)
            {
                LatestVersion = null;
            }
        }
    }
}
