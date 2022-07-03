using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities;

namespace ScreenCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        globalKeyboardHook gkh = new globalKeyboardHook();
        List<FullScreenWindow> CapturedWindows;

        public MainWindow()
        {
            InitializeComponent();
        }

        private bool Window_KeyDown(object sender, Key e)
        {
            return false;
        }

        private bool Window_KeyUp(object sender, Key e)
        {
            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Test a = new Test();
            //a.Show();
            //return;

            CapturedWindows = new List<FullScreenWindow>();

            System.Drawing.Rectangle totalSize = System.Drawing.Rectangle.Empty;
            var allScreens = Screen.AllScreens;
            foreach (Screen s in Screen.AllScreens)
                totalSize = System.Drawing.Rectangle.Union(totalSize, s.Bounds);
            FullScreenWindow aScreen = new FullScreenWindow();
            aScreen.Width = totalSize.Width + 20;
            aScreen.Height = totalSize.Height;

            aScreen.Left = totalSize.Left - 7;
            aScreen.Top = totalSize.Top - 7;

            CapturedWindows.Add(aScreen);

            aScreen.Closed += AScreen_Closed;
            //aScreen.Topmost = true;
            aScreen.Show();
        }

        private void AScreen_Closed(object sender, EventArgs e)
        {
            foreach(var window in CapturedWindows)
            {
                if(sender != window)
                {
                    window.Close();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gkh.addKeyToWatch (Key.A);
            gkh.addKeyToWatch(Key.B);
            gkh.KeyDown += new globalKeyboardHook.HookedKeyEventHandler(Window_KeyDown);
            gkh.KeyUp += new globalKeyboardHook.HookedKeyEventHandler(Window_KeyUp);
            
            gkh.hook();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            gkh.unhook();
        }
    }
}
