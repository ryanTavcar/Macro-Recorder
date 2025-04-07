using Names.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Names.Views
{
    /// <summary>
    /// Interaction logic for ConsoleWindow.xaml
    /// </summary>
    public partial class ConsoleWindow : Window
    {

        private StringBuilder logBuilder = new StringBuilder();

        public ConsoleWindow()
        {
            InitializeComponent();

            // Subscribe to log events
            LoggerService.Instance.LogAdded += OnLogAdded;

            // Ensure we unsubscribe when the window is closed
            this.Closed += (s, e) => LoggerService.Instance.LogAdded -= OnLogAdded;
        }

        // Drag window
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnLogAdded(string message)
        {
            // Make sure we're on the UI thread
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => OnLogAdded(message));
                return;
            }

            logBuilder.AppendLine(message);
            consoleBox.Text = logBuilder.ToString();
            consoleBox.ScrollToEnd();
        }
    }
}
