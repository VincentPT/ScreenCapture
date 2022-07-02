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

namespace ScreenCapture
{
    /// <summary>
    /// Interaction logic for SimpleDrawingCanvas.xaml
    /// </summary>
    public partial class SimpleDrawingCanvas : UserControl
    {
        public SimpleDrawingCanvas()
        {
            InitializeComponent();
        }

        bool mouseDrag = false;
        Point firstDown;
        Rectangle currentCreatingElement = null;
        private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseDrag = true;
            firstDown = e.GetPosition(DrawingCanvas);
        }

        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var currentPos = e.GetPosition(DrawingCanvas);
            if(mouseDrag)
            {
                var rect = new System.Windows.Rect(firstDown, currentPos);
                if (currentCreatingElement == null)
                {
                    Rectangle rectangle = new Rectangle();
                    DrawingCanvas.Children.Add(rectangle);
                    currentCreatingElement = rectangle;
                }

                currentCreatingElement.Width = rect.Width;
                currentCreatingElement.Height = rect.Height;
                Canvas.SetLeft(currentCreatingElement, rect.X);
                Canvas.SetTop(currentCreatingElement, rect.Y);
            }
        }

        private void DrawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseDrag = false;
            currentCreatingElement = null;
        }
    }
}
