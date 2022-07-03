﻿using System;
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
            
            DrawingMode = EditMode.None;
        }

        bool mouseDrag = false;
        Point firstDown;
        UIElement currentCreatingElement = null;
        UIElementBuilder _elementBuilder = null; 

        public enum EditMode
        {
            None,
            Comment,
            Text,
            Rect,
            Line
        }

        private void CreateUIElement()
        {
            switch (DrawingMode)
            {
                case EditMode.Line:
                    currentCreatingElement = new Line();
                    break;
                case EditMode.Comment:
                    break;
                case EditMode.Text:
                    break;
                case EditMode.Rect:
                    break;
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DrawingMode == EditMode.None) return;

            mouseDrag = true;
            firstDown = e.GetPosition(DrawingCanvas);
            e.Handled = true;

            switch (DrawingMode)
            {
                case EditMode.Line:
                    _elementBuilder = new LineElementBuilder();
                    break;
                case EditMode.Comment:
                    _elementBuilder = new CommentElementBuilder();
                    break;
                case EditMode.Text:
                    _elementBuilder = new TextElementBuilder();
                    break;
                case EditMode.Rect:
                    _elementBuilder = new RectElementBuilder();
                    break;
            }
            currentCreatingElement = _elementBuilder.CreateElement(this);
            _elementBuilder.UpdateElement(currentCreatingElement, firstDown, firstDown);

            DrawingCanvas.Children.Add(currentCreatingElement);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (DrawingMode == EditMode.None) return;

            var currentPos = e.GetPosition(DrawingCanvas);
            if(mouseDrag)
            {
                _elementBuilder.UpdateElement(currentCreatingElement, firstDown, currentPos);
                e.Handled = true;
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DrawingMode == EditMode.None) return;
            currentCreatingElement = null;
            mouseDrag = false;
            e.Handled = true;
        }

        private EditMode _editMode;
        public EditMode DrawingMode
        {
            get => _editMode;
            set
            {
                _editMode = value;
                if (_editMode == EditMode.None)
                {
                    CanvasContainer.Cursor = Cursors.Arrow;
                }
                else
                {
                    CanvasContainer.Cursor = Cursors.Cross;
                }
            }
        }

        private void Button_Coment_Click(object sender, RoutedEventArgs e)
        {
            DrawingMode = EditMode.Comment;
        }
        private void Button_Text_Click(object sender, RoutedEventArgs e)
        {
            DrawingMode = EditMode.Text;
        }
        private void Button_Rect_Click(object sender, RoutedEventArgs e)
        {
            DrawingMode = EditMode.Rect;
        }

        private void Button_Line_Click(object sender, RoutedEventArgs e)
        {
            DrawingMode = EditMode.Line;
        }

        abstract class UIElementBuilder
        {
            public abstract UIElement CreateElement(SimpleDrawingCanvas context);
            public abstract void UpdateElement(UIElement element, Point firstPoint, Point currentPoint);
        }

        abstract class ShapeBuilder : UIElementBuilder
        {
            public override UIElement CreateElement(SimpleDrawingCanvas context)
            {
                Shape shape = CreateShapeElement(context);
                shape.StrokeThickness = 1;
                shape.Stroke = new SolidColorBrush(Colors.Red);
                return shape;
            }

            public abstract Shape CreateShapeElement(SimpleDrawingCanvas context);
        }

        class LineElementBuilder : ShapeBuilder
        {
            public override Shape CreateShapeElement(SimpleDrawingCanvas context)
            {
                return new Line();
            }
            public override void UpdateElement(UIElement element, Point firstPoint, Point currentPoint)
            {
                ((Line)element).X1 = firstPoint.X;
                ((Line)element).Y1 = firstPoint.Y;
                ((Line)element).X2 = currentPoint.X;
                ((Line)element).Y2 = currentPoint.Y;
            }
        }

        class RectElementBuilder : ShapeBuilder
        {
            public override Shape CreateShapeElement(SimpleDrawingCanvas context)
            {
                return new Rectangle();
            }
            public override void UpdateElement(UIElement element, Point firstPoint, Point currentPoint)
            {
                var rect = new System.Windows.Rect(firstPoint, currentPoint);
                Shape shape = (Shape)element;
                shape.Width = rect.Width;
                shape.Height = rect.Height;
                Canvas.SetLeft(shape, rect.X);
                Canvas.SetTop(shape, rect.Y);
            }
        }

        class TextElementBuilder : UIElementBuilder
        {
            public override UIElement CreateElement(SimpleDrawingCanvas context)
            {
                StackPanel horizontal = new StackPanel { Orientation = Orientation.Horizontal };
                TextBox textBox = new TextBox
                {
                    AcceptsReturn = true,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "Type your text here",
                    Background = new SolidColorBrush(Colors.Transparent)
                };

                textBox.SelectAll();

                horizontal.Children.Add(textBox);

                return horizontal;
            }
            public override void UpdateElement(UIElement element, Point firstPoint, Point currentPoint)
            {
                Canvas.SetLeft(element, firstPoint.X);
                Canvas.SetTop(element, firstPoint.Y);

                ((StackPanel)element).Children[0].Focus();
            }
        }

        class CommentElementBuilder : UIElementBuilder
        {
            public override UIElement CreateElement(SimpleDrawingCanvas context)
            {
                StackPanel horizontal = new StackPanel { Orientation = Orientation.Horizontal };                
                TextBox textBox = new TextBox
                {
                    AcceptsReturn = true,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = "Type your text here",
                    Background = new SolidColorBrush(Colors.Transparent)
                };

                textBox.SelectAll();

                horizontal.Children.Add(textBox);

                return horizontal;
            }
            public override void UpdateElement(UIElement element, Point firstPoint, Point currentPoint)
            {
                Canvas.SetLeft(element, firstPoint.X);
                Canvas.SetTop(element, firstPoint.Y);

                ((StackPanel)element).Children[0].Focus();
            }
        }
    }
}
