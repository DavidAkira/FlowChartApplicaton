using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlowChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Shape> shapeList = new List<Shape>();
        private Enums.SelectedShape currentOption;
        private bool isShapeMoving;
        private bool isMakingLine;
        private Polyline polyLine;
        private Polyline polySegment;
        private Brush chosenColor;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void CanvasChart_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Tar musposition
            Point pt = e.GetPosition((Canvas) sender);

            //Använder HitTest() för att se om användaren klickade på ett item i canvas
            HitTestResult result = VisualTreeHelper.HitTest(CanvasChart, pt);

            // Tar bort den visuella träffen från canvas.
            if (result != null)
            {
                CanvasChart.Children.Remove(result.VisualHit as Shape);
            }
        }

        private void CanvasChart_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            chosenColor = Brushes.Red;
            Point pt = e.GetPosition((Canvas)sender);
            Shape shapeToRender = null;
            switch (currentOption)
            {
                case Enums.SelectedShape.Circle:  
                    shapeToRender = new Ellipse()
                    {
                        Stroke = Brushes.Black,
                        Fill = chosenColor,
                        Height = 30,
                        Width = 50,
                    };
                    break;
                case Enums.SelectedShape.Rectangle:
                    shapeToRender = new Rectangle()
                    {
                        Stroke = Brushes.Black,
                        Fill = chosenColor,
                        Height = 30,
                        Width = 50,
                        RadiusX = 10,
                        RadiusY = 10
                    };
                    break;
                case Enums.SelectedShape.Diamond:
                    shapeToRender = new Polygon
                    {
                        Points = new PointCollection()
                        {
                            new Point(10, 25),
                            new Point(25, 10),
                            new Point(40, 25),
                            new Point(25, 40)
                        },
                        Stroke = new SolidColorBrush(Colors.Black),
                        Fill = chosenColor
                    };
                    break;
                case Enums.SelectedShape.Line:
                    polyLine = new Polyline() {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };
                    polyLine.Points.Add(pt);
                    CanvasChart.Children.Add(polyLine);
                    polySegment = new Polyline { StrokeThickness = 2 };
                    polySegment.Stroke = Brushes.Red;
                    polySegment.Points.Add(pt);
                    polySegment.Points.Add(pt);
                    CanvasChart.Children.Add(polySegment);
                    shapeList.Add(polyLine);
                    isMakingLine = true;
                    break;
                case Enums.SelectedShape.Text:
                    break;
                case Enums.SelectedShape.Move:
                    HitTestResult result = VisualTreeHelper.HitTest(CanvasChart, pt);
                    if (result != null)
                    {
                        var unknownShape = result.VisualHit as Shape;
                        if (unknownShape != null)
                        {
                            unknownShape.CaptureMouse();
                            isShapeMoving = true;                
                        }
                    }                  
                    break;
                    
                default:
                    return;
            }

            if (shapeToRender != null)
            {
                double left = pt.X - (shapeToRender.ActualWidth/2);
                double top = pt.Y - (shapeToRender.ActualHeight/2);
                Canvas.SetLeft(shapeToRender, left);
                Canvas.SetTop(shapeToRender,top);
                CanvasChart.Children.Add(shapeToRender);
                shapeList.Add(shapeToRender);
            }
        }

        private void CanvasChart_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isShapeMoving)
            {
                
                Point pt = e.GetPosition((Canvas) sender);
                HitTestResult result = VisualTreeHelper.HitTest(CanvasChart, pt);
                if (result != null)
                {
                    var unknownShape = result.VisualHit as Shape;
                    if (unknownShape != null)
                    {
                        unknownShape.ReleaseMouseCapture();
                        isShapeMoving = false;                    
                    }
                }
            }
            if (isMakingLine && polyLine != null)
            {
                polySegment.Points[1] = e.GetPosition(CanvasChart);

                var distance = (polySegment.Points[0] - polySegment.Points[1]).Length;
                if (distance >= 20)
                {
                    polyLine.Points.Add(polySegment.Points[1]);
                    polySegment.Points[0] = polySegment.Points[1];
                    isMakingLine = false;    
                    polyLine = null;
                }
                else
                {
                    if (polyLine.Points.Count < 2)
                    {
                      CanvasChart.Children.Remove(polyLine);   
                    }
                    polyLine = null;
                    polySegment.Points.Clear();
                    CanvasChart.Children.Remove(polySegment);

                }
            }
        }

        private void CanvasChart_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (polyLine != null)
            {
                polySegment.Points[1] = e.GetPosition(CanvasChart);

                var distance = (polySegment.Points[0] - polySegment.Points[1]).Length;
                polySegment.Stroke = distance >= 20 ? Brushes.Green : Brushes.Red;
            }
            if (!isShapeMoving)
            {
                return;
            }
            Point pt = e.GetPosition((Canvas) sender);
            HitTestResult result = VisualTreeHelper.HitTest(CanvasChart, pt);
            if (result != null)
            {
                var unknownShape = result.VisualHit as Shape;
                if (unknownShape != null)
                {
                    // Centrerar geometri till musen
                    double left = pt.X - (unknownShape.ActualWidth / 2);
                    double top = pt.Y - (unknownShape.ActualHeight / 2);
                    //Sätter ny position
                    Canvas.SetLeft(unknownShape, left);
                    Canvas.SetTop(unknownShape, top);        
                }
            }

        }

        private void BtnClear_OnClick(object sender, RoutedEventArgs e)
        {
            CanvasChart.Children.Clear();
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RbCircleOption_OnClick(object sender, RoutedEventArgs e)
        {
            currentOption = Enums.SelectedShape.Circle;
        }

        private void RbRectangleOption_OnClick(object sender, RoutedEventArgs e)
        {
            currentOption = Enums.SelectedShape.Rectangle;
        }

        private void RbDiamondOption_OnClick(object sender, RoutedEventArgs e)
        {
            currentOption = Enums.SelectedShape.Diamond;
        }

        private void RbLineOption_OnClick(object sender, RoutedEventArgs e)
        {
            currentOption = Enums.SelectedShape.Line;
        }

        private void RbTextOption_OnClick(object sender, RoutedEventArgs e)
        {
            currentOption = Enums.SelectedShape.Text;
        }

        private void RbMoveOption_OnClick(object sender, RoutedEventArgs e)
        {
            currentOption = Enums.SelectedShape.Move;
        }
    }
}
