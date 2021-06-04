using System;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;
using System.Windows.Input;
using System.ComponentModel;

namespace RFUI
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Forms.NotifyIcon notifyIcon;
        public UpdatePage _UpdatePage;

        public MainWindow()
        {
            InitializeComponent();

            //Создаем NotifyIcon
            CreateNotifyIcon();

            Pages();
            Frame0.Navigate(_UpdatePage);
        }

        void Pages()
        {
            _UpdatePage = new UpdatePage();
        }

        public void CheckUpdates()
        {

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //для двиганья окна
            DragMove();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Кнопки
            Button ClickedButton = (Button)sender;
            if ((string)ClickedButton.Tag == "Close")
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
            else if ((string)ClickedButton.Tag == "Minimize")
            {
                this.WindowState = WindowState.Minimized;
            }
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
            _StripMenuItemAppName.Image = Properties.Resources.rfulogo0525;

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
    }
}
