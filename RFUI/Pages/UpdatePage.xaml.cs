using System.Windows.Controls;
using System.Windows;

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
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button ClickedButton = (Button)sender;
            if ((string)ClickedButton.Tag == "Install")
            {
                ((MainWindow)Window.GetWindow(this))._ProgressBar.Visibility = Visibility.Visible;
            }
            else if ((string)ClickedButton.Tag == "Update")
            {
                ((MainWindow)Window.GetWindow(this))._ProgressBar.Visibility = Visibility.Visible;
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
