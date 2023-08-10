using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TestKot
{
    public partial class MainWindow : Window
    {
        public void LogsCreator(string logText)
        {
            LogBox.Text += logText + "\n";
        }
        public void LogsCreatorAsync(string logTextAsync)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
              new Action<string>(LogsCreator),
              logTextAsync);
        }
    }
}
