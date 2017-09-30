using HY_Utility.Data;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace HY_Utility
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string storageUrl = @"https://teamhy.github.io/";
        public static string ModsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games\Binding of Isaac Afterbirth+ Mods";

        public string TempFolderPath;

        public Brush DefaultProgressBarBrush;

        private delegate void CSafeSetValue(Int32 value);

        private CSafeSetValue cssv;

        private WebClient wc;
        private bool nowDownloading;

        private int totalFiles;
        private int filesExtracted;

        public MainWindow()
        {
            cssv = new CSafeSetValue(CrossSafeSetValueMethod);

            wc = new WebClient();

            InitializeComponent();
        }

        public void ExtractZipfile(string sourceFilePath, string targetPath)
        {
            var readOptions = new ReadOptions() { Encoding = Encoding.GetEncoding("euc-kr") };

            using (ZipFile zip = ZipFile.Read(sourceFilePath, readOptions))
            {
                totalFiles = zip.Count;
                filesExtracted = 0;

                /// Unzipping 될때 이벤트 핸들러를 등록 (현재 진행 상황을 위해)
                zip.ExtractProgress += zip_ExtractProgress;

                /// Unzipping
                zip.ExtractAll(targetPath, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        private void zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.EventType == ZipProgressEventType.Extracting_BeforeExtractEntry)
            {
                filesExtracted++;
                CrossSafeSetValueMethod((int)(100 * filesExtracted / totalFiles));
            }
            else if (e.EventType == ZipProgressEventType.Extracting_AfterExtractAll)
            {
                nowDownloading = false;
                //btnStart.IsEnabled = true;
                //btnReload.IsEnabled = true;

                MessageBox.Show("ChaosGreedier 설치 완료!", "설치 완료", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void VersionInfoUpdate()
        {
            VersionUtility.CheckVersion();

            if (ModData.ChaosGreedier.CurrentVersion != null)
                lblCurrentVersion.Content = ModData.ChaosGreedier.CurrentVersion;
            else
                lblCurrentVersion.Content = "N/A";

            if (ModData.ChaosGreedier.LatestVersion != null)
                lblLatestVersion.Content = ModData.ChaosGreedier.LatestVersion;
            else
                lblLatestVersion.Content = "N/A";
        }

        private void StartFileDownload(String remoteAddress, String localPath)
        {
            if (nowDownloading)
            {
                MessageBox.Show("이미 다운로드가 진행 중입니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (String.IsNullOrEmpty(remoteAddress))
            {
                MessageBox.Show("주소가 입력되지 않았습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // 파일이 저장될 위치를 저장한다.
            String fileName = String.Format(@"{0}\{1}", localPath, System.IO.Path.GetFileName(remoteAddress) + ".temp");

            // 폴더가 존재하지 않는다면 폴더를 생성한다.
            if (!System.IO.Directory.Exists(localPath))
                System.IO.Directory.CreateDirectory(localPath);

            try
            {
                // C 드라이브 밑의 downloadFiles 폴더에 파일 이름대로 저장한다.
                wc.DownloadFileAsync(new Uri(remoteAddress), fileName);

                // 다운로드 중이라는걸 알리기 위한 값을 설정하고,
                // 프로그레스바의 크기를 0으로 만든다.
                prgInstall.Value = 0;
                prgInstall.Foreground = Brushes.Gold;
                nowDownloading = true;
                btnStart.IsEnabled = false;
                btnReload.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().FullName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void CrossSafeSetValueMethod(Int32 value)
        {
            if (!prgInstall.Dispatcher.CheckAccess())
                prgInstall.Dispatcher.Invoke(cssv, value);
            else
                prgInstall.Value = value;
        }

        private void MainFormLoad(object sender, RoutedEventArgs e)
        {
            TempFolderPath = System.IO.Path.GetTempPath() + @"\" + Assembly.GetEntryAssembly().GetName().Name;
            DefaultProgressBarBrush = prgInstall.Foreground;
            VersionInfoUpdate();

            // 이벤트를 연결한다.
            wc.DownloadFileCompleted += new AsyncCompletedEventHandler(FileDownloadCompleted);
            wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(FileDownloadProgressChanged);
        }

        void FileDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            // e.BytesReceived
            //   받은 데이터의 크기를 저장합니다.

            // e.TotalBytesToReceive
            //   받아야 할 모든 데이터의 크기를 저장합니다.

            // 프로그레스바의 최대 크기가 정해지지 않은 경우,
            // 받아야 할 최대 데이터 량으로 설정한다.
            //if (!setBaseSize)
            //{
            //    CrossSafeSetMaximumMethod((int)e.TotalBytesToReceive);
            //    setBaseSize = true;
            //}

            // 받은 데이터 량을 나타낸다.
            CrossSafeSetValueMethod((int)(100 * e.BytesReceived / e.TotalBytesToReceive));

            // 받은 데이터 / 받아야할 데이터 (퍼센트) 로 나타낸다.
            //CrossSafeSetTextMethod(String.Format("{0:N0} / {1:N0} ({2:P})", e.BytesReceived, e.TotalBytesToReceive, (Double)e.BytesReceived / (Double)e.TotalBytesToReceive));
        }

        void FileDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            prgInstall.Foreground = DefaultProgressBarBrush;
            CrossSafeSetValueMethod(0);
            //nowDownloading = false;
            btnStart.IsEnabled = true;
            btnReload.IsEnabled = true;

            var zipFilePath = String.Format(@"{0}\ChaosGreedier_{1}.zip", TempFolderPath, ModData.ChaosGreedier.LatestVersion);
            var zipTempFilePath = zipFilePath + ".temp";

            if (File.Exists(zipTempFilePath))
            {
                if (File.Exists(zipFilePath))
                    File.Delete(zipFilePath);

                File.Move(zipTempFilePath, zipFilePath);
                var thread = new Thread(new ThreadStart(InstallChaosGreedier));
                thread.Start();
            }
            else
            {
                MessageBox.Show("파일이 다운로드 되지 않았습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
        }

        private void InstallChaosGreedier()
        {
            ModUtility.RemoveMod("chaosgreedier");
            ExtractZipfile(String.Format(@"{0}\ChaosGreedier_{1}.zip", TempFolderPath, ModData.ChaosGreedier.LatestVersion), ModsPath + @"\chaosgreedier");
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            VersionInfoUpdate();

            StartFileDownload(ModData.ChaosGreedier.LatestUrl, TempFolderPath);
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            VersionInfoUpdate();
        }
    }
}
