using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HY_Utill
{
    class VersionUtility
    {
        public static string LatestVersion { get; set; }
        public static string CurrentVersion { get; set; }

        public static void CheckVersion()
        {
            if (System.IO.File.Exists(MainWindow.ModsPath + @"\chaosgreedier\metadata.xml"))
            {
                XmlDocument currentXml = new XmlDocument();

                currentXml.Load(MainWindow.ModsPath + @"\chaosgreedier\metadata.xml");

                XmlNodeList currentXmlNodeList = currentXml.SelectNodes("/metadata");

                foreach (XmlNode xn in currentXmlNodeList)
                {
                    CurrentVersion = xn["version"].InnerText;
                }
            }
            else
            {
                CurrentVersion = null;
            }

            System.IO.Path.GetTempFileName();

            XmlDocument latestXml = new XmlDocument();

            latestXml.Load(MainWindow.storageUrl + "versiondata.xml");

            XmlNodeList latestXmlNodeList = latestXml.SelectNodes("/versiondata/chaosgreedier");

            foreach (XmlNode xn in latestXmlNodeList)
            {
                //Changelog = xn["changelog"].InnerText;

                LatestVersion = xn["version"].InnerText;
            }
        }
    }
}
