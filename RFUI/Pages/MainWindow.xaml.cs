using System.Windows;
using System.Windows.Controls;

namespace RFUI
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button ClickedButton = (Button)sender;
            if ((string)ClickedButton.Tag == "Close")
            {
                if(Properties.Settings.Default.CanClose == true)
                {
                    this.Close();
                }
                else
                {
                    this.Hide();
                }
            }
            else if ((string)ClickedButton.Tag == "Minimize")
            {
                this.Hide();
            }
        }
    }
}
