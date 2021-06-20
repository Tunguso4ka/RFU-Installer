using System.Reflection;
using System.Windows.Controls;

namespace RFUI
{
    /// <summary>
    /// Interakční logika pro AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
            AboutTextBox.Text = "RFUI (Random Fights Updater Installer)\nVersion:" + Assembly.GetExecutingAssembly().GetName().Version +"\nMade by: Kira Kosova\nTwitter: @tunguso4ka\nGitHub: tunguso4ka\nI <3 Stef\nThank you!";
        }
    }
}
