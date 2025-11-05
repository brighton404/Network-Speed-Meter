using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace TaskbarSpeedMeter
{
    public partial class App : System.Windows.Application
    {
        private TaskbarOverlay overlay;
        private UserSettings settings;
        private NotifyIcon trayIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            settings = UserSettings.Load();
            overlay = new TaskbarOverlay();

            // restore saved position
            if (settings.LastLeft > 0 && settings.LastTop > 0)
            {
                overlay.Left = settings.LastLeft;
                overlay.Top = settings.LastTop;
            }
            else
            {
                overlay.PositionAtTaskbar();
            }

            overlay.Show();

            InitializeTrayIcon();
        }

        private void InitializeTrayIcon()
        {
            var iconPath = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory,  "Assets", "speedmeter.ico");

            trayIcon = new NotifyIcon
            {
                Icon = new Icon("Assets/speedmeter.ico"), // you can replace with a custom icon
                Visible = true,
                Text = "Taskbar Speed Meter"
            };

            var menu = new ContextMenuStrip();
            menu.Items.Add("Show/Hide Widget", null, (s, e) => ToggleOverlay());
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Exit", null, (s, e) => ExitApp());

            trayIcon.ContextMenuStrip = menu;
            trayIcon.DoubleClick += (s, e) => ToggleOverlay();
        }

        private void ToggleOverlay()
        {
            if (overlay == null) return;

            if (overlay.Visibility == Visibility.Visible)
                overlay.Hide();
            else
                overlay.Show();
        }

        private void ExitApp()
        {
            settings.LastLeft = overlay.Left;
            settings.LastTop = overlay.Top;
            settings.Save();

            trayIcon.Visible = false;
            trayIcon.Dispose();

            overlay.Close();
            Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            }
            catch { }

            if (overlay != null)
            {
                settings.LastLeft = overlay.Left;
                settings.LastTop = overlay.Top;
                settings.Save();
                overlay.Close();
            }

            base.OnExit(e);
        }
    }
}
