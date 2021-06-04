using System.Windows.Controls;
using System.Windows;
using System.Threading.Tasks;
using System;

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

            if (RFUStatus == 0 )
            {
                InstallBtn.IsEnabled = false;
            }
            else if (RFUStatus == 1)
            {
                InstallBtn.Tag = "Update";
                InstallBtn.Content = "Update";
                InstallBtn.ToolTip = "Update";
            }

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
            InstallBtn.Content = "Install";
            DeleteBtn.IsEnabled = true;
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
        }
    }
}
