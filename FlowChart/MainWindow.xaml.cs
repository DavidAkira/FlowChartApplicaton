using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlowChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Shape> shapeList = new List<Shape>();
        private bool isShapeMoving;
        private bool isMakingLine;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnRectangle_Click(object sender, RoutedEventArgs e)
        {
            Rectangle rekt = new Rectangle();
            rekt.Stroke = new SolidColorBrush(Colors.Blue);
            rekt.Fill = new SolidColorBrush(Colors.LightBlue);
            rekt.Height = 30;
            rekt.Width = 50;
            Canvas.SetLeft(rekt, 10);
            Canvas.SetTop(rekt, 10);
            CanvasChart.Children.Add(rekt);
            shapeList.Add(rekt);

        }

        private void btnEllipse_Click(object sender, RoutedEventArgs e)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Stroke = new SolidColorBrush(Colors.Blue);
            ellipse.Fill = new SolidColorBrush(Colors.LightBlue);
            ellipse.Height = 30;
            ellipse.Width = 50;
            Canvas.SetLeft(ellipse, 10);
            Canvas.SetTop(ellipse, 60);
            CanvasChart.Children.Add(ellipse);
            shapeList.Add(ellipse);
        }

        private void btnRomb_Click(object sender, RoutedEventArgs e)
        {
            Polygon rhomb = new Polygon
            {
                Points = new PointCollection()
                {
                    new Point(10,25),
                    new Point(25,10),
                    new Point(40,25),
                    new Point(25,40)
                },
                Stroke = new SolidColorBrush(Colors.Blue),
                Fill = new SolidColorBrush(Colors.LightBlue),

        };

            //RotateTransform transform = new RotateTransform(45);
            //rhomb.RenderTransform = transform;
            //Canvas.SetLeft(rhomb, 25);
            //Canvas.SetTop(rhomb, 110);
            CanvasChart.Children.Add(rhomb);
            shapeList.Add(rhomb);
        }

        private void BtnLine_OnClick(object sender, RoutedEventArgs e)
        {
            if (isMakingLine != true)
            {
                isMakingLine = true;
                btnLine.IsEnabled = false;
            }
            else
            {
                isMakingLine = false;
            }




        }

        private void CanvasChart_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // First, get the X,Y location of where the user clicked.
            Point pt = e.GetPosition((Canvas) sender);

            // Use the HitTest() method of VisualTreeHelper to see if the user clicked
            // on an item in the canvas.
            HitTestResult result = VisualTreeHelper.HitTest(CanvasChart, pt);

            // If the result is not null, they DID click on a shape!
            if (result != null)
            {
                // Get the underlying shape clicked on, and remove it from
                // the canvas.
                CanvasChart.Children.Remove(result.VisualHit as Shape);
            }
        }

        private void CanvasChart_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isMakingLine != true)
            {
                isShapeMoving = true;
                Point pt = e.GetPosition((Canvas) sender);
                HitTestResult result = VisualTreeHelper.HitTest(CanvasChart, pt);
                if (result != null)
                {
                    var unknownShape = result.VisualHit as Shape;
                    unknownShape.CaptureMouse();
                
                }               
            }

            Point pt = e.GetPosition((Canvas)sender);
            Line line = new Line
            {
                Stroke = new SolidColorBrush(Colors.Black),
                

            };

            Canvas.SetLeft(line, 10);
            Canvas.SetRight(line, 130);
            CanvasChart.Children.Add(line);
            shapeList.Add(line);
            //HitTestResult result = VisualTreeHelper.HitTest(CanvasChart, pt);
            //if (result != null)
            //{
            //    // Get the underlying shape clicked on, and remove it from
            //    // the canvas.

            //    var mysteryShape = result.VisualHit as Shape;

            //    Canvas.SetLeft(mysteryShape, pt.X);
            //    Canvas.SetTop(mysteryShape, pt.Y);
            //}

        }

        private void CanvasChart_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isShapeMoving = false;
            Point pt = e.GetPosition((Canvas) sender);
            HitTestResult result = VisualTreeHelper.HitTest(CanvasChart, pt);
            if (result != null)
            {
                var unknownShape = result.VisualHit as Shape;
                unknownShape.ReleaseMouseCapture();
            }
        }

        private void CanvasChart_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!isShapeMoving)
            {
                return;
            }
            Point pt = e.GetPosition((Canvas) sender);
            HitTestResult result = VisualTreeHelper.HitTest(CanvasChart, pt);
            if (result != null)
            {
                var unknownShape = result.VisualHit as Shape;
                // Centrerar geometri till musen
                double left = pt.X - (unknownShape.ActualWidth/2);
                double top = pt.Y - (unknownShape.ActualHeight/2);
                //Sätter ny position
                Canvas.SetLeft(unknownShape, left);
                Canvas.SetTop(unknownShape, top);

            }

        }
    
    }
}
