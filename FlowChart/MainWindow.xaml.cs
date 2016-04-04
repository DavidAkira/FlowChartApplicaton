using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace FlowChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Enums.SelectedShape currentOption = Enums.SelectedShape.Move;
        private bool isShapeMoving;
        private bool isMakingLine;
        private Polyline polyLine;
        private Polyline polySegment;
        private SolidColorBrush chosenColor;
        private Grid unknownGrid = null;
        private Shape unknownShape = null;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void CanvasChart_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Tar musposition
            Point pt = e.GetPosition(CanvasChart);

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
            //Tar fram ett hexadecimalt värde ifrån färgväljaren och converterar till en Brush
            chosenColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(clrPicker.SelectedColorText));
            Point pt = e.GetPosition(CanvasChart);
            Grid gridToRender = null;
            switch (currentOption)
            {
                case Enums.SelectedShape.Circle:
                    gridToRender = new Grid
                    {   
                        Background = Brushes.Transparent,
                        Height = 50,
                        Width = 80
                    };
                    gridToRender.Children.Add(new Ellipse
                    {
                        Fill = chosenColor,
                        Height = 50,
                        Width = 80,                       
                    });
                    gridToRender.Children.Add(new TextBox
                    {
                        BorderBrush = Brushes.Transparent,
                        Background = chosenColor,
                        MaxHeight = 50,
                        MaxWidth = 80,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        Text = "Ellipse"
                    });
                    break;
                case Enums.SelectedShape.Rectangle:
                    gridToRender = new Grid
                    {
                        Background = Brushes.Transparent,
                        Height = 50,
                        Width = 80

                    };
                    gridToRender.Children.Add(new Rectangle()
                    {
                        Fill = chosenColor,
                        Height = 50,
                        Width = 80,
                        RadiusX = 10,
                        RadiusY = 10
                    });
                    gridToRender.Children.Add(new TextBox
                    {
                        BorderBrush = Brushes.Transparent,
                        Background = chosenColor,
                        MaxHeight = 50,
                        MaxWidth = 80,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        Text = "Rektangel"
                    });
                    break;
                case Enums.SelectedShape.Diamond:
                    gridToRender = new Grid
                    {
                        Background = Brushes.Transparent,
                        Height = 80,
                        Width = 80
                    };
                    gridToRender.Children.Add(new Polygon
                    {
                        Points = new PointCollection()
                        {
                            new Point(0, 40),
                            new Point(40, 0),
                            new Point(80, 40),
                            new Point(40, 80)
                        },
                        Fill = chosenColor
                    });
                    gridToRender.Children.Add(new TextBox
                    {
                        BorderBrush = Brushes.Transparent,
                        Background = chosenColor,
                        MaxHeight = 100,
                        MaxWidth = 80,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        Text = "Romb"
                    });

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
                    isMakingLine = true;
                    break;
                case Enums.SelectedShape.Move:
                    HitTestResult result = VisualTreeHelper.HitTest(CanvasChart, pt);
                    if (result != null)
                    {
                        unknownGrid = result.VisualHit as Grid;
                        if (unknownGrid != null)
                        {       
                            unknownGrid.CaptureMouse();
                            isShapeMoving = true;
                        }
                        unknownShape = result.VisualHit as Shape;
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
            if (gridToRender != null)
            {
                Canvas.SetLeft(gridToRender, pt.X);
                Canvas.SetTop(gridToRender, pt.Y);
                CanvasChart.Children.Add(gridToRender);
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
            Point pt = e.GetPosition(CanvasChart);
            if (isShapeMoving)
            {
                
                if (unknownGrid != null)
                {
                    Canvas.SetTop(unknownGrid, pt.Y - unknownGrid.Height / 2);
                    Canvas.SetLeft(unknownGrid, pt.X - unknownGrid.Width / 2);
                }
                if (unknownShape != null)
                {
                    var MovableShape = unknownShape.Parent  as Grid;
                    Canvas.SetTop(MovableShape, pt.Y - MovableShape.Height / 2);
                    Canvas.SetLeft(MovableShape, pt.X - MovableShape.Width / 2);
                }      
            }
        }

        private void CanvasChart_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(CanvasChart);
            if (isShapeMoving)
            {        
                    if (unknownGrid != null)
                    {
                        unknownGrid.ReleaseMouseCapture();
                        isShapeMoving = false;
                    }
                    if (unknownShape != null)
                    {
                        unknownShape.ReleaseMouseCapture();
                        isShapeMoving = false;
                    }
            }

            if (isMakingLine && polyLine != null)
            {
                polySegment.Points[1] = e.GetPosition(CanvasChart);
                //kontrollerar att linjen är längre än 20 annars ritas den ej ut.
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

        private void RbMoveOption_OnClick(object sender, RoutedEventArgs e)
        {
            currentOption = Enums.SelectedShape.Move;
        }
    }
}
