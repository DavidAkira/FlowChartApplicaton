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
        private SolidColorBrush chosenColor;
        private Grid unknownGrid = null;
        private Shape unknownShape = null;
        private bool firstLine = true;
        private Point firstLinePosition;
        private Point secondLinePosition;

        private Point currentPosition;
        private Grid movableGrid;

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
                CanvasChart.Children.Remove(result.VisualHit as Grid);
            }
        }

        private void CanvasChart_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Tar fram ett hexadecimalt värde ifrån färgväljaren och converterar till en Brush
            chosenColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(clrPicker.SelectedColorText));
            Point pt = e.GetPosition(CanvasChart);
            Grid gridToRender = null;
            HitTestResult result = VisualTreeHelper.HitTest(CanvasChart, pt);
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
                    unknownShape = result.VisualHit as Shape;
                    if (unknownShape != null)
                    {
                        Grid gridOfShape = unknownShape.Parent as Grid;
                        if (firstLine)
                        {
                            firstLinePosition.X = Canvas.GetLeft(gridOfShape) + (gridOfShape.Width / 2);
                            firstLinePosition.Y = Canvas.GetTop(gridOfShape) + (gridOfShape.Height / 2);
                            firstLine = false;
                        }
                        else
                        {
                            secondLinePosition.X = Canvas.GetLeft(gridOfShape) + (gridOfShape.Width / 2);
                            secondLinePosition.Y = Canvas.GetTop(gridOfShape) + (gridOfShape.Height / 2);
                            Line newLine = new Line
                            {
                                Stroke = Brushes.Black,
                                StrokeThickness = 2,
                                X1 = firstLinePosition.X,
                                X2 = secondLinePosition.X,
                                Y1 = firstLinePosition.Y,
                                Y2 = secondLinePosition.Y
                            };
                            Canvas.SetZIndex(newLine, -10);
                            CanvasChart.Children.Add(newLine);
                            firstLine = true;
                        }
                    }
                    break;
                case Enums.SelectedShape.Move:
                    if (result != null)
                    {
                        unknownGrid = result.VisualHit as Grid;
                        if (unknownGrid != null)
                        {
                            currentPosition = new Point
                            {
                                X = Canvas.GetLeft(unknownGrid),
                                Y = Canvas.GetTop(unknownGrid)
                            };    
                            unknownGrid.CaptureMouse();
                            isShapeMoving = true;
                        }
                        unknownShape = result.VisualHit as Shape;
                        
                        if (unknownShape != null)
                        {
                            movableGrid = unknownShape.Parent as Grid;

                            currentPosition = new Point()
                            {
                                X = Canvas.GetLeft(movableGrid),
                                Y = Canvas.GetTop(movableGrid)
                            };

                            

                            //movableGrid = unknownShape.Parent as Grid;

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
            if (!isShapeMoving)
            {
                return;
            }
            Point pt = e.GetPosition(CanvasChart);
            if (isShapeMoving)
            {
                if (unknownGrid != null)
                {
                    //set new grid position
                    Canvas.SetTop(unknownGrid, pt.Y - unknownGrid.Height / 2);
                    Canvas.SetLeft(unknownGrid, pt.X - unknownGrid.Width / 2);

                    //hitta alla lines i canvas som ligger i samma koordinater som denna griden och flytta dem samtidigt
                    foreach (var item in CanvasChart.Children)
                    {
                        Line line = item as Line;

                        if(line != null)
                        {
                            if (line.X1 == currentPosition.X && line.Y1 == currentPosition.Y)
                            {
                                line.X1 = pt.X;
                                line.Y1 = pt.Y;
                            }
                            else if (line.X2 == currentPosition.X && line.Y2 == currentPosition.Y)
                            {
                                line.X2 = pt.X;
                                line.Y2 = pt.Y;
                            }
                        }  
                    }
                    currentPosition = pt; 
                }
                if (unknownShape != null)
                {
                    //Canvas.SetTop(unknownGrid, pt.Y - unknownGrid.Height / 2);
                    //Canvas.SetLeft(unknownGrid, pt.X - unknownGrid.Width / 2);

                    //set new grid position
                    //Grid MovableGrid = unknownShape.Parent  as Grid;
                    Canvas.SetTop(movableGrid, pt.Y - movableGrid.Height / 2);
                    Canvas.SetLeft(movableGrid, pt.X - movableGrid.Width / 2);

                    System.Console.WriteLine("hgjgfhjv");

                    //hitta alla lines i canvas som ligger i samma koordinater som denna griden och flytta dem samtidigt
                    foreach (var item in CanvasChart.Children)
                    {
                        Line line = item as Line;

                        if (line != null)
                        {
                            System.Console.WriteLine("Current Grid pos: " + new Point(currentPosition.X, currentPosition.Y).ToString());
                            System.Console.WriteLine("new Grid pos: " + new Point(Canvas.GetLeft(movableGrid),
                                                                                    Canvas.GetTop(movableGrid)).ToString());
                            System.Console.WriteLine("line1 pos: " + new Point(line.X1 - (movableGrid.Width / 2), line.Y1 - (movableGrid.Height / 2)).ToString());
                            System.Console.WriteLine("line2 pos: " + new Point(line.X2, line.Y2).ToString());

                            //System.Console.WriteLine("Line pos: " + new Point(line.X1, line.Y1).ToString());

                            Point linePos = new Point(currentPosition.X + (movableGrid.Width / 2), currentPosition.Y + (movableGrid.Height / 2));

                            System.Console.WriteLine("line pos adjusted: " + linePos.ToString());

                            if (line.X1 == linePos.X && line.Y1 == linePos.Y)
                            {
                                System.Console.WriteLine("2");
                                line.X1 = pt.X;
                                line.Y1 = pt.Y;
                            }
                            else if (line.X2 == linePos.X && line.Y2 == linePos.Y)
                            {
                                System.Console.WriteLine("3");
                                line.X2 = pt.X;
                                line.Y2 = pt.Y;
                            }
                        }
                    }
                    currentPosition = new Point(Canvas.GetLeft(movableGrid), Canvas.GetTop(movableGrid));
                }      
            }
        }

        private void CanvasChart_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isShapeMoving)
            {        
                if (unknownGrid != null)
                {
                    unknownGrid.ReleaseMouseCapture();
                    isShapeMoving = false;
                    unknownGrid = null;
                }
                if (unknownShape != null)
                {
                    unknownShape.ReleaseMouseCapture();
                    isShapeMoving = false;
                    unknownShape = null;
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
