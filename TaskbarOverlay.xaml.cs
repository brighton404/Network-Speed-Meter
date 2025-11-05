using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Forms;

namespace TaskbarSpeedMeter
{
    public partial class TaskbarOverlay : Window {
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private NetworkInterface activeInterface;
        private long previousBytesSent;
        private long previousBytesReceived;
        private DateTime previousTime;

        public TaskbarOverlay() {
            InitializeComponent();
            Loaded += OnLoaded;

            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Tick += (_, __) => UpdateSpeed();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            FindActiveNetworkInterface();
            ResetCounters();

            PositionAtTaskbar();
            timer.Start();
        }

        private void FindActiveNetworkInterface() {
            // Here i am picking the first active network, non-loopback network adapter
            activeInterface = NetworkInterface
                .GetAllNetworkInterfaces()
                .FirstOrDefault(n =>
                    n.OperationalStatus == OperationalStatus.Up &&
                    n.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    n.GetIPv4Statistics().BytesReceived > 0);

            if (activeInterface == null) {
                UploadText.Text = "No net";
                DownloadText.Text = "No net";
            }
        }

        private void ResetCounters() {
            if (activeInterface != null) {
                var stats = activeInterface.GetIPv4Statistics();
                previousBytesSent = stats.BytesSent;
                previousBytesReceived = stats.BytesReceived;
                previousTime = DateTime.UtcNow;
            }
        }

        private void UpdateSpeed() {
            if (activeInterface == null) {
                FindActiveNetworkInterface();
                return;
            }

            var now = DateTime.UtcNow;
            var elapsed = (now - previousTime).TotalSeconds;
            if (elapsed <= 0) return;

            var stats = activeInterface.GetIPv4Statistics();
            long newBytesSent = stats.BytesSent;
            long newBytesReceived = stats.BytesReceived;

            double uploadSpeed = (newBytesSent - previousBytesSent) / 1024.0 / elapsed;
            double downloadSpeed = (newBytesReceived - previousBytesReceived) / 1024.0 / elapsed;

            previousBytesSent = newBytesSent;
            previousBytesReceived = newBytesReceived;
            previousTime = now;

            UploadText.Text = $"{uploadSpeed:F1} kB/s";
            DownloadText.Text = $"{downloadSpeed:F1} kB/s";
        }

        public void PositionAtTaskbar()
        {
            var screen = Screen.PrimaryScreen.WorkingArea;
            Left = screen.Right - Width - 10;
            Top = screen.Bottom - Height - 4;
        }
        
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {   // Allow dragging the window with left mouse button
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }

    }
}
