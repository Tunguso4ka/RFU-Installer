using System.Windows;
using System.Windows.Controls;

namespace RFUI
{
    /// <summary>
    /// Interakční logika pro MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Кнопки 
            Button ClickedButton = (Button)sender;
            if ((string)ClickedButton.Tag == "Exit")
            {
                if (Properties.Settings.Default.CanClose == true)
                {
                    ((MainWindow)Window.GetWindow(this)).KillNotifyIcon();
                    ((MainWindow)Window.GetWindow(this)).Close();
                }
                else
                {
                    ((MainWindow)Window.GetWindow(this)).WindowState = WindowState.Minimized;
                }
            }
            else if ((string)ClickedButton.Tag == "App")
            {
                //((MainWindow)Window.GetWindow(this)).Frame0.Navigate();
            }
            else if ((string)ClickedButton.Tag == "ChangeTheme")
            {

            }
        }
    }
}
