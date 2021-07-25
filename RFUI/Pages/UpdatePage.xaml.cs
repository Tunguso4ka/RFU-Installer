using System.Windows.Controls;
using System.Windows;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace RFUI
{
    /// <summary>
    /// Interakční logika pro UpdatePage.xaml
    /// </summary>
    public partial class UpdatePage : Page
    {
        public UpdatePage()
        {
            InitializeComponent();

            int RFUStatus = Properties.Settings.Default.RFUStatus;

            if (RFUStatus == -2 )
            {
                DeleteBtn.Visibility = Visibility.Hidden;
                InstallBtn.Tag = "Install";
                InstallBtn.Content = "Install";
                InstallBtn.ToolTip = "Install";
            }
            else if (RFUStatus == 1)
            {
                InstallBtn.Tag = "Update";
                InstallBtn.Content = "Update";
                InstallBtn.ToolTip = "Update";
            }
            else
            {
                InstallBtn.Tag = "Open";
                InstallBtn.Content = "Open";
                InstallBtn.ToolTip = "Open";
            }

            /*
            if (GameStatus == -2)
            {
                StatusTextBlock.Text = "Status: Not installed.";
                InstallBtn.Content = "⬇💾Install";
                InstallBtn.ToolTip = "Install";
                DeleteBtn.Visibility = Visibility.Hidden;
            }
            else if (GameStatus == 1)
            {
                StatusTextBlock.Text = "Status: Update found.";
                InstallBtn.Content = "🆕Update";
                InstallBtn.ToolTip = "Update";
                DeleteBtn.Visibility = Visibility.Visible;
            }
            else
            {
                StatusTextBlock.Text = "Status: Installed.";
                InstallBtn.Content = "🎮Play";
                InstallBtn.ToolTip = "Play";
                DeleteBtn.Visibility = Visibility.Visible;
            }
            */

            VersionTextBlock.Text = "Version: " + Properties.Settings.Default.InstalledVersion + "(" + Properties.Settings.Default.NewVersion + ")";
        }

        async void InfoUpdate()
        {
            ((MainWindow)Window.GetWindow(this)).Installing();
            while (((MainWindow)Window.GetWindow(this)).IsInstalling == true)
            {
                DownloadingSpeed.Text = Convert.ToString(((MainWindow)Window.GetWindow(this)).RecievedBytes / 1000) + " kB/s ";
                await Task.Delay(500);
            }
            InstallBtn.Content = "Open";
            InstallBtn.IsEnabled = true;
            DeleteBtn.IsEnabled = true;
        }

        async void OpenApp()
        {
            try
            {
                InstallBtn.IsEnabled = false;
                DeleteBtn.IsEnabled = false;

                InstallBtn.Content = "Running";

                Process RFUProcess = new Process();
                RFUProcess.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\RFUpdater.exe";
                RFUProcess.StartInfo.Arguments = "";
                RFUProcess.Exited += new EventHandler(ProcessExited);
                RFUProcess.Start();

                //Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RFUpdater\RFUpdater.exe"
            }
            catch
            {
                InstallBtn.IsEnabled = true;
                DeleteBtn.IsEnabled = true;

                InstallBtn.Content = "Open";
            }

        }

        public void ProcessExited(object sender, System.EventArgs e)
        {
            InstallBtn.IsEnabled = true;
            DeleteBtn.IsEnabled = true;

            InstallBtn.Content = "Open";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button ClickedButton = (Button)sender;
            if ((string)ClickedButton.Tag == "Install")
            {
                ((MainWindow)Window.GetWindow(this))._ProgressBar.Visibility = Visibility.Visible;
                InstallBtn.Content = "Installing";
                InstallBtn.IsEnabled = false;
                DeleteBtn.IsEnabled = false;
                InfoUpdate();
            }
            else if ((string)ClickedButton.Tag == "Update")
            {
                ((MainWindow)Window.GetWindow(this))._ProgressBar.Visibility = Visibility.Visible;
                InstallBtn.Content = "Updating";
                InstallBtn.IsEnabled = false;
                DeleteBtn.IsEnabled = false;
                InfoUpdate();
            }
            else if ((string)ClickedButton.Tag == "Delete")
            {
                ((MainWindow)Window.GetWindow(this)).DeleteRFU();
                InstallBtn.Content = "Install";
                InstallBtn.IsEnabled = true;
                DeleteBtn.IsEnabled = false;
            }
            else if ((string)ClickedButton.Tag == "Info")
            {
                if(InfoBorder.Visibility == Visibility.Collapsed)
                {
                    ClickedButton.ToolTip = "Hide info";
                    InfoBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    ClickedButton.ToolTip = "Show info";
                    InfoBorder.Visibility = Visibility.Collapsed;
                }
            }
            else if ((string)ClickedButton.Tag == "Open")
            {
                OpenApp();
            }
        }
    }
}
