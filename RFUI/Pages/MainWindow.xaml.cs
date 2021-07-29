using System;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;
using System.Windows.Input;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace RFUI
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Forms.NotifyIcon notifyIcon;
        public UpdatePage _UpdatePage;
        public MenuPage _MenuPage;
        public AboutPage _AboutPage;
        public bool IsInstalling;
        public double RecievedBytes;

        //Ссылка на местоположение на компьютере
        string UpdateInfoPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\Update.txt";
        //Ссылка на файл с информацией
        string UpdateInfoUrl = "https://drive.google.com/uc?export=download&id=1oKyTppE7V8E-Q0UF0_SXNmW3diQ0QbLJ"; //https://drive.google.com/uc?export=download&id=1oKyTppE7V8E-Q0UF0_SXNmW3diQ0QbLJ
        //Ссылка на файл с информацией
        string RFUUrl;
        string ZipPath;
        string AppPath;

        bool IsInstalled = true;

        public MainWindow()
        {
            InitializeComponent();

            //проверяем из правильного ли места запущено приложение
            CheckLocation();

            //Создаем NotifyIcon
            CreateNotifyIcon();

            Properties.Settings.Default.RFUPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\";
            Properties.Settings.Default.Save();

            CheckUpdates();

            Pages();
            Frame0.Navigate(_UpdatePage);
            Frame1.Navigate(_MenuPage);
        }

        void CheckLocation()
        {
            if (Environment.CurrentDirectory != Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater")
            {
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\RFUI.exe"))
                {
                    File.Copy(Environment.CurrentDirectory + @"\RFUI.exe", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\RFUI.exe", true);
                }
            }
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\RFUpdater.exe"))
            {
                Properties.Settings.Default.InstalledVersion = "0";
                Properties.Settings.Default.Save();
                IsInstalled = false;
                //MessageBox.Show("df", "a");
            }
        }

        void Pages()
        {
            _UpdatePage = new UpdatePage();
            _MenuPage = new MenuPage();
        }

        public void CheckUpdates()
        {
            try
            {
                if (File.Exists(UpdateInfoPath))
                {
                    File.Delete(UpdateInfoPath);
                }

                WebClient WebClient = new WebClient();
                WebClient.DownloadFile(UpdateInfoUrl, UpdateInfoPath);
                using (StreamReader StreamReader = new StreamReader(UpdateInfoPath))
                {
                    Properties.Settings.Default.NewVersion = StreamReader.ReadLine();
                    RFUUrl = StreamReader.ReadLine();
                    StreamReader.Dispose();
                }

                File.Delete(UpdateInfoPath);

                //MessageBox.Show("CheckUpdates", "b");

                Version InstalledVersion;
                Version NewVersion;

                try
                {
                    InstalledVersion = new Version(Properties.Settings.Default.InstalledVersion);
                    NewVersion = new Version(Properties.Settings.Default.NewVersion);
                }
                catch
                {
                    InstalledVersion = new Version("0.0.0.0");
                    NewVersion = new Version("0.0.0.0");
                }

                //MessageBox.Show("CheckUpdates", "c");

                //проверяем наличие и версию файла на компьютере
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\RFUpdater.exe"))
                {
                    //MessageBox.Show("CheckUpdates", "d");
                    FileVersionInfo _InstalledVersion = FileVersionInfo.GetVersionInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\RFUpdater.exe");
                    InstalledVersion = new Version(_InstalledVersion.ProductVersion);
                    Properties.Settings.Default.InstalledVersion = Convert.ToString(InstalledVersion);
                    Properties.Settings.Default.Save();
                }

                //MessageBox.Show("CheckUpdates", "e");

                int RFUStatus = -2;

                if (IsInstalled == false)
                {
                    RFUStatus = -2;
                }
                else
                {
                    switch (NewVersion.CompareTo(InstalledVersion))
                    {
                        case 0:
                            RFUStatus = 0; //такая же
                            break;
                        case 1:
                            RFUStatus = 1; //новее
                            break;
                        case -1:
                            RFUStatus = -1; //старше
                            break;
                    }
                    //MessageBox.Show(RFUStatus + "", "b");
                }
                //MessageBox.Show(RFUStatus + "", "c");

                Properties.Settings.Default.RFUStatus = RFUStatus;
                Properties.Settings.Default.Save();
            }
            catch 
            {
                MessageBox.Show("CheckUpdates", "a");
            }
        }

        public void Installing()
        {
            IsInstalling = true;
            Properties.Settings.Default.CanClose = false;

            _ProgressBar.IsIndeterminate = false;

            WebClient WebClient = new WebClient();

            if (!Directory.Exists(Properties.Settings.Default.RFUPath))
            {
                Directory.CreateDirectory(Properties.Settings.Default.RFUPath);
            }

            ZipPath = Properties.Settings.Default.RFUPath + @"\RFUpdater.zip";
            AppPath = Properties.Settings.Default.RFUPath + @"\RFUpdater.exe";

            if (File.Exists(ZipPath))
            {
                File.Delete(ZipPath);
            }

            if (File.Exists(AppPath))
            {
                File.Delete(AppPath);
            }

            Uri UpdateUri = new Uri(RFUUrl);
            WebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            WebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            WebClient.DownloadFileAsync(UpdateUri, ZipPath);

            //InstallBtn.IsEnabled = false;
            _ProgressBar.Visibility = Visibility.Visible;
            //_ProgressTextBox.Visibility = Visibility.Visible;
            //DownSpeedTextBlock.Visibility = Visibility.Visible;
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //ProgressBar0.Value = e.ProgressPercentage;
            _ProgressBar.Value = e.ProgressPercentage;
            //_ProgressTextBox.Text = e.ProgressPercentage + "%";
            //DownSpeedTextBlock.Text = "Bytes received: " + e.BytesReceived;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                try
                {
                    MessageBox.Show("Complete");
                    Properties.Settings.Default.RFUStatus = 0;

                    ZipFile.ExtractToDirectory(ZipPath, Properties.Settings.Default.RFUPath);
                    File.Delete(ZipPath);
                }
                catch
                {
                    MessageBox.Show("Error: Can't download this app, try later.", "Error");
                }
            }
            //InstallBtn.IsEnabled = true;
            //DeleteBtn.Visibility = Visibility.Visible;
            //ProgressBar0.Visibility = Visibility.Hidden;
            _ProgressBar.Visibility = Visibility.Collapsed;
            //_ProgressTextBox.Visibility = Visibility.Collapsed;
            //DownSpeedTextBlock.Visibility = Visibility.Collapsed;

            Properties.Settings.Default.CanClose = true;
            Properties.Settings.Default.InstalledVersion = Properties.Settings.Default.NewVersion;
            Properties.Settings.Default.Save();

            IsInstalling = false;
        }

        public void DeleteRFU()
        {
            AppPath = Properties.Settings.Default.RFUPath + @"\RFUpdater.exe";

            if (File.Exists(AppPath))
            {
                File.Delete(AppPath);
            }

            Properties.Settings.Default.InstalledVersion = "0.0.0.0";
            Properties.Settings.Default.Save();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //для двиганья окна
            DragMove();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Кнопки 
            Button ClickedButton = (Button)sender;
            if ((string)ClickedButton.Tag == "CloseBtn")
            {
                if(Properties.Settings.Default.CanClose == true)
                {
                    KillNotifyIcon();
                    this.Close();
                }
                else
                {
                    this.WindowState = WindowState.Minimized;
                }
            }
            else if ((string)ClickedButton.Tag == "MinimizeBtn")
            {
                this.WindowState = WindowState.Minimized;
            }
            else if ((string)ClickedButton.Tag == "MaximizeBtn")
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                    ClickedButton.Content = "";
                }
                else
                {
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.WindowState = WindowState.Maximized;
                    ClickedButton.Content = "";
                    this.WindowStyle = WindowStyle.None;
                }
                
            }
            else if ((string)ClickedButton.Tag == "ShowMenuBtn")
            {
                if(Frame1.Visibility == Visibility.Visible)
                {
                    ClickedButton.ToolTip = "Show Menu";
                    ClickedButton.Content = "";
                    Frame1.Visibility = Visibility.Collapsed;
                }
                else if (this.Width >= 600 && Frame1.Visibility == Visibility.Collapsed)
                {
                    ClickedButton.ToolTip = "Hide Menu";
                    ClickedButton.Content = "";
                    Frame1.Visibility = Visibility.Visible;
                }
            }

            //
        }
        private void CreateNotifyIcon()
        {
            notifyIcon = new Forms.NotifyIcon(new Container());

            Forms.ContextMenuStrip _ContextMenuStrip = new Forms.ContextMenuStrip();

            Forms.ToolStripMenuItem _StripMenuItemAppName = new Forms.ToolStripMenuItem();

            _ContextMenuStrip.Items.AddRange(
                new Forms.ToolStripMenuItem[]
                {
                    _StripMenuItemAppName,
                    new Forms.ToolStripMenuItem("Exit", null, new EventHandler(ExitClicked))
                }
            );

            _StripMenuItemAppName.Text = "RFU Installer";
            _StripMenuItemAppName.Enabled = false;
            _StripMenuItemAppName.Image = Properties.Resources.rfui0729;

            notifyIcon.Icon = Properties.Resources.rfulogo0525_lcM_icon;
            notifyIcon.ContextMenuStrip = _ContextMenuStrip;
            notifyIcon.Text = "RFU Installer";
            notifyIcon.Visible = true;

            notifyIcon.MouseDown += new Forms.MouseEventHandler(NotifyIconClicked);

        }

        [STAThread]
        private void NotifyIconClicked(object sender, Forms.MouseEventArgs e)
        {
            if (e.Button == Forms.MouseButtons.Left)
            {
                this.WindowState = WindowState.Normal;
                this.ShowInTaskbar = true;
            }
        }
        private void ExitClicked(object sender, EventArgs e)
        {
            KillNotifyIcon();
            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }
        public void KillNotifyIcon()
        {
            notifyIcon.Dispose();
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Maximized;
                MaximizeBtn.Content = "";
                this.WindowStyle = WindowStyle.None;
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(this.Width <= 600)
            {
                ShowMenuBtn.ToolTip = "Show Menu";
                ShowMenuBtn.Content = "";
                Frame1.Visibility = Visibility.Collapsed;
            }
        }
    }
}
