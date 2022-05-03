using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gkh.addKeyToWatch (Key.A);
            gkh.addKeyToWatch(Key.B);
            gkh.KeyDown += new globalKeyboardHook.HookedKeyEventHandler(Window_KeyDown);
            gkh.KeyUp += new globalKeyboardHook.HookedKeyEventHandler(Window_KeyUp);
        }
    }
}
