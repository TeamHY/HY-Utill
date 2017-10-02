using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HY_Updater
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private WebClient webClient;

        private string _UtilityProcessName;

        private string _remoteAddress;
        private string _localPath;

        private string _tempFolderPath;
        private string _tempFilePath;

        private bool _nowUpdating = true;
        private bool _setMaximum;

        public MainWindow()
        {
            webClient = new WebClient();

            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {            
            _tempFolderPath = System.IO.Path.GetTempPath() + Assembly.GetEntryAssembly().GetName().Name + @"\";

            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(UpdateProgressChanged);
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(UpdateCompleted);

            if (App.Args[0] != "exit")
            {
                _UtilityProcessName = App.Args[0];
                _remoteAddress = App.Args[1];
                _localPath = App.Args[2];

                try
                {
                    KillUtility();
                    UpdateStart();
                }
                catch
                {
                    MessageBox.Show("업데이트 실패!", this.Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    RunUtility();
                }
            }
            else
            {
                MessageBox.Show("인수 오류!", this.Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);

                _nowUpdating = false;
                this.Close();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_nowUpdating)
            {
                MessageBoxResult result = MessageBox.Show("업데이트를 취소하시겠습니까?", this.Title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateStart()
        {
            try
            {
                // 파일이 저장될 위치를 저장한다.
                _tempFilePath = String.Format(@"{0}{1}", _tempFolderPath, System.IO.Path.GetFileName(_remoteAddress) + ".temp");

                // 폴더가 존재하지 않는다면 폴더를 생성한다.
                if (!Directory.Exists(_tempFolderPath))
                    Directory.CreateDirectory(_tempFolderPath);

                try
                {
                    webClient.DownloadFileAsync(new Uri(_remoteAddress), _tempFilePath);

                    //prgUpdate.Value = 0;
                    _nowUpdating = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, e.GetType().FullName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        private void UpdateProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (!_setMaximum)
            {
                PrgUpdate.Maximum = (int)e.TotalBytesToReceive;
                _setMaximum = true;
            }

            PrgUpdate.Value = (int)e.BytesReceived;
        }

        private void UpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                KillUtility();
                File.Delete(_localPath);
                File.Move(_tempFilePath, _localPath);

                _nowUpdating = false;
                RunUtility();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().FullName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void RunUtility()
        {
            MessageBoxResult result = MessageBox.Show("업데이트가 완료되었습니다.\r\n실행하시겠습니까?", this.Title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                Process process = new Process();
                process.StartInfo.FileName = _localPath;
                process.Start();
            }

            this.Close();
        }

        private void KillUtility()
        {
            Process[] processList = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(_localPath));

            if (processList.Length == 0)
                return;

            foreach (Process process in processList)
            {
                process.Kill();
            }
        }
    }
}
