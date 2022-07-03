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
using System.Windows.Shapes;

namespace ScreenCapture
{
    /// <summary>
    /// Interaction logic for FullScreenWindow.xaml
    /// </summary>
    public partial class FullScreenWindow : Window
    {
        BitmapSource capturedScreenshot;
        Border theMask;
        SimpleDrawingCanvas theWorkingArea;
        public FullScreenWindow()
        {
            InitializeComponent();
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {                
                this.Close();
            }
        }


        private void Window_Initialized(object sender, EventArgs e)
        {
            capturedScreenshot = ScreenCaptureA.CopyAllScreen();
            Image backgroundImage = new Image
            {
                Width = capturedScreenshot.Width,
                Height = capturedScreenshot.Height,
                Name = "backgroundImage",
                Source = capturedScreenshot,
            };

            DrawingBoard.Children.Add(backgroundImage);

            SolidColorBrush mask = new SolidColorBrush
            {
                Color = Colors.Aqua,
                Opacity = 0.5
            };


            Border border = new Border
            {
                Background = mask,
                Width = backgroundImage.Width,
                Height = backgroundImage.Height
            };

            DrawingBoard.Children.Add(border);

            theMask = border;
            //{
            //    theWorkingArea = new SimpleDrawingCanvas();
            //    theWorkingArea.Width = 200;
            //    theWorkingArea.Height = 200;
            //    Canvas.SetLeft(theWorkingArea, 100);
            //    Canvas.SetTop(theWorkingArea, 100);

            //    DrawingBoard.Children.Add(theWorkingArea);

            //    UpdateClip();
            //}
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            sender = null;
        }

        bool mouseDrag = false;
        bool workingAreaCreateDone = false;
        Point firstDown;
        private void DrawingBoard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            firstDown = e.GetPosition(DrawingBoard);
            mouseDrag = true;
        }

        private void DrawingBoard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseDrag = false;
            workingAreaCreateDone = theWorkingArea != null;
            if(theWorkingArea != null)
            {
                theWorkingArea.IsHitTestVisible = true;
            }
        }

        private void UpdateClip()
        {
            var x = Canvas.GetLeft(theWorkingArea);
            var y = Canvas.GetTop(theWorkingArea);

            RectangleGeometry totalDrawingExtents = new RectangleGeometry(new Rect(0, 0, theMask.Width, theMask.Height));

            // This is the geometry of a "hole"
            var holeRect = new System.Windows.Rect(x, y, theWorkingArea.Width, theWorkingArea.Height);
            RectangleGeometry hole = new RectangleGeometry(holeRect);

            // Combine two objects, to get a total bounding box with a hole in the middle
            CombinedGeometry combined = new CombinedGeometry(GeometryCombineMode.Exclude, totalDrawingExtents, hole);

            theMask.Clip = combined;
        }

        private void DrawingBoard_MouseMove(object sender, MouseEventArgs e)
        {            
            var currentPos = e.GetPosition(DrawingBoard);

            if (workingAreaCreateDone && mouseDrag)
            {
                var translate = currentPos - firstDown;
                var x = Canvas.GetLeft(theWorkingArea);
                var y = Canvas.GetTop(theWorkingArea);

                if(currentPos.X > x && currentPos.X < x + theWorkingArea.Width && currentPos.Y > y && currentPos.Y < y + theWorkingArea.Height)
                {
                    return;
                }

                Canvas.SetLeft(theWorkingArea, x + translate.X);
                Canvas.SetTop(theWorkingArea, y + translate.Y);

                firstDown = currentPos;

                UpdateClip();

                e.Handled = true;
            }
            else if(mouseDrag)
            {
                var holeRect = new System.Windows.Rect(firstDown, currentPos);

                if(theWorkingArea == null)
                {
                    theWorkingArea = new SimpleDrawingCanvas();
                    theWorkingArea.IsHitTestVisible = false;
                    DrawingBoard.Children.Add(theWorkingArea);
                }
                Canvas.SetLeft(theWorkingArea, holeRect.Left);
                Canvas.SetTop(theWorkingArea, holeRect.Top);
                theWorkingArea.Width = holeRect.Width;
                theWorkingArea.Height = holeRect.Height;

                UpdateClip();

                e.Handled = true;
            }
        }
    }
}
