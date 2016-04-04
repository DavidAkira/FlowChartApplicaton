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
        private Enums.SelectedShape currentSelectedOption = Enums.SelectedShape.Move;
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
        private void cnvMain_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Tar musposition relativ till Canvas
            Point pt = e.GetPosition(cnvMain);

            //Använder HitTest() för att se om användaren klickade på ett item i canvas
            HitTestResult result = VisualTreeHelper.HitTest(cnvMain, pt);

            // Tar bort den visuella träffen från canvas vid högerklick.
            if (result != null)
            {
                Shape removableShape = result.VisualHit as Shape;
                if (removableShape != null)
                {
                    var removableGrid = removableShape.Parent as Grid;            
                    cnvMain.Children.Remove(result.VisualHit as Shape);
                    cnvMain.Children.Remove(removableGrid);                
                }    
            }
        }

        private void cnvMain_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Tar fram ett hexadecimalt värde ifrån färgväljaren och converterar till en Brush
            chosenColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(clrPicker.SelectedColorText));
            
            Point pt = e.GetPosition(cnvMain);
            Grid gridToRender = null;
            HitTestResult result = VisualTreeHelper.HitTest(cnvMain, pt);
            switch (currentSelectedOption)
            {
                //Skapar de olika formerna
                case Enums.SelectedShape.Ellipse:
                    gridToRender = new Grid
                    {       
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
                                StrokeThickness = 5,
                                X1 = firstLinePosition.X,
                                X2 = secondLinePosition.X,
                                Y1 = firstLinePosition.Y,
                                Y2 = secondLinePosition.Y
                            };
                            Canvas.SetZIndex(newLine, -10);
                            newLine.IsHitTestVisible = false;
                            cnvMain.Children.Add(newLine);
                            firstLine = true;
                        }
                    }
                    break;
                case Enums.SelectedShape.Move:
                    //Greppar en shape på canvasen och sätter igång ett tracking event på musen för att kunna flytta omkring shapen på canvasen.
                    if (result != null)
                    {
                        unknownShape = result.VisualHit as Shape; 
                        if (unknownShape != null)
                        {
                            movableGrid = unknownShape.Parent as Grid;
                            if (movableGrid != null)
                            {
                                currentPosition = new Point()
                                {
                                    X = Canvas.GetLeft(movableGrid),
                                    Y = Canvas.GetTop(movableGrid)
                                };
                                movableGrid.CaptureMouse();
                                isShapeMoving = true;     
                            }
                        }
                    }
                    break;  
                default:
                    return;
            }
            if (gridToRender != null)
            {
                //Ritar ut formen på canvasen.
                Canvas.SetLeft(gridToRender, pt.X);
                Canvas.SetTop(gridToRender, pt.Y);
                cnvMain.Children.Add(gridToRender);
            }
        }
        private void cnvMain_OnMouseMove(object sender, MouseEventArgs e)
        {
            //Om inte Move funktionen är aktiverad, return.
            if (!isShapeMoving)
            {
                return;
            }
            Point pt = e.GetPosition(cnvMain);
            if (isShapeMoving)
            {
                if (unknownShape != null)
                {
                    //Sätter den flyttade shapen till en ny position på canvas.
                    Canvas.SetTop(movableGrid, pt.Y - movableGrid.Height / 2);
                    Canvas.SetLeft(movableGrid, pt.X - movableGrid.Width / 2);

                    //hitta alla lines i canvas som ligger i samma koordinater som denna griden och flytta dem samtidigt
                    foreach (var item in cnvMain.Children)
                    {
                        Line line = item as Line;
                        if (line != null)
                        {
                            //justerar positionen på var den hittar linjen
                            Point linePos = new Point(currentPosition.X + (movableGrid.Width / 2), currentPosition.Y + (movableGrid.Height / 2));

                            if (line.X1 == linePos.X && line.Y1 == linePos.Y)
                            {
                                line.X1 = pt.X;
                                line.Y1 = pt.Y;
                            }
                            else if (line.X2 == linePos.X && line.Y2 == linePos.Y)
                            {
                                line.X2 = pt.X;
                                line.Y2 = pt.Y;
                            }
                        }
                    }
                    currentPosition = new Point(Canvas.GetLeft(movableGrid), Canvas.GetTop(movableGrid));
                }      
            }
        }

        private void cnvMain_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Avslutar move funktionen.
            if (isShapeMoving)
            {        
                if (movableGrid != null)
                {
                    movableGrid.ReleaseMouseCapture();
                    isShapeMoving = false;
                }
            }
        }


        private void BtnClear_OnClick(object sender, RoutedEventArgs e)
        {
            cnvMain.Children.Clear();
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RbEllipseOption_OnClick(object sender, RoutedEventArgs e)
        {
            currentSelectedOption = Enums.SelectedShape.Ellipse;
        }

        private void RbRectangleOption_OnClick(object sender, RoutedEventArgs e)
        {
            currentSelectedOption = Enums.SelectedShape.Rectangle;
        }

        private void RbDiamondOption_OnClick(object sender, RoutedEventArgs e)
        {
            currentSelectedOption = Enums.SelectedShape.Diamond;
        }

        private void RbLineOption_OnClick(object sender, RoutedEventArgs e)
        {
            currentSelectedOption = Enums.SelectedShape.Line;
        }

        private void RbMoveOption_OnClick(object sender, RoutedEventArgs e)
        {
            currentSelectedOption = Enums.SelectedShape.Move;
        }
    }
}
